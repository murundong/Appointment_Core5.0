using AutoMapper;
using CoreApplication.Services.IServices;
using CoreBaseClass;
using CoreEntityFramework;
using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using CoreEntityFramework.Repository;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Appoint.Application.Services
{
    public class DoorUsersCardsService : IDoorUsersCardsService
    {
        private IRepository<App_DbContext, DoorUsersCards> _repository;
        private IRepository<App_DbContext, DoorUsersAppoints> _repositoryAppoints;
        private IRepository<App_DbContext, Doors> _repositoryDoors;
        private IRepository<App_DbContext, DoorUsers> _repositoryDoorUsers;
        private IRepository<App_DbContext, View_UserCardsInfoOutput> _repositorySql;
        private IRepository<App_DbContext, View_Appoint_UsersCardsInfo> _repositoryAppointDoorUserCardsInfoSql;

        public DoorUsersCardsService(
            IRepository<App_DbContext, DoorUsersCards> repository,
            IRepository<App_DbContext, DoorUsersAppoints> repositoryAppoints,
            IRepository<App_DbContext, Doors> repositoryDoors,
            IRepository<App_DbContext, DoorUsers> repositoryDoorUsers,
            IRepository<App_DbContext, View_UserCardsInfoOutput> repositorySql,
            IRepository<App_DbContext, View_Appoint_UsersCardsInfo> repositoryAppointDoorUserCardsInfoSql
            )
        {
            _repository = repository;
            _repositoryAppoints = repositoryAppoints;
            _repositoryDoors = repositoryDoors;
            _repositoryDoorUsers = repositoryDoorUsers;
            _repositorySql = repositorySql;
            _repositoryAppointDoorUserCardsInfoSql = repositoryAppointDoorUserCardsInfoSql;
        }



        public View_InitialUserCardsInfoOutput GetUserLst_Door(int doorid, string nick)
        {
            View_InitialUserCardsInfoOutput return_res = new View_InitialUserCardsInfoOutput();
            string sql = @"select A.id,A.door_id,A.uid,[door_role]=A.role,[door_remark]=A.remark,B.open_id,B.nick_name,B.avatar,B.gender,B.role,B.tel,B.initial,B.real_name,B.birthday from [dbo].[DoorUsers] A
		    left join [dbo].[UserInfos] B
		    on A.uid = B.uid
		    where A.door_id = @doorid and (nick_name like @nick or A.remark like @nick) order by initial;";
            var sqlParm = new SqlParameter[] {
                new SqlParameter("@doorid",doorid),
                new SqlParameter("@nick",$"%{nick}%"),
            };
            var query = _repository.dbContext.Set<View_UserCardsInfoOutput>().FromSqlRaw(sql, sqlParm).ToList();  // _repositorySql.ExecuteSqlQuery(sql, sqlParm)?.ToList();
            List<string> LstLetters = query.Select(s => s.initial)?.Distinct()?.ToList();
            return_res.initials = LstLetters;
            if (LstLetters?.Count > 0)
            {
                LstLetters.ForEach(s =>
                {
                    View_InitialUserCardsInfoItemOutput item = new View_InitialUserCardsInfoItemOutput()
                    {
                        initial = s,
                        uinfos = query.Where(p => p.initial == s)?.ToList()
                    };
                    return_res.uinfos.Add(item);
                });
            }
            return return_res;
        }



        public List<View_LstUserAllCardsOutput> GetUserALlCards(string openid, Enum_CardStatus cardStatus)
        {
            List<View_LstUserAllCardsOutput> res = new List<View_LstUserAllCardsOutput>();

            string sql = @"select  A.id,A.door_id,[uid]=A.du_id,A.cid,A.ctype,A.card_sttime,A.card_edtime,A.effective_time,A.limit_week_time,A.limit_day_time,A.is_freeze,A.freeze_edtime
                        ,C.card_name,C.card_desc,
                        B.door_img from [dbo].[DoorUsersCards] A
                        left join [dbo].[Doors] B
                        on A.door_id = B.id
                        left join [dbo].[CardTemplate] C
                        on A.cid = C.id
                        left join [dbo].[UserInfos] E
                        on A.uid = E.uid
                        where  A.cid is not null and E.open_id = @openid and A.is_delete=0
                      ";
            switch (cardStatus)
            {
                case Enum_CardStatus.Expired:
                    sql += " and card_edtime <= GETDATE()";
                    break;
                case Enum_CardStatus.Freezed:
                    sql += " and is_freeze=1";
                    break;
                default:
                    sql += " and (card_edtime > GETDATE() or card_edtime is null) and is_freeze =0";
                    break;
            }
            sql += "  order by A.id desc ;";
            var sqlParm = new SqlParameter[] {
                new SqlParameter("@openid",openid),
            };
            List<View_UserCardsInfoOutput> uCardLst = _repository.dbContext.Set<View_UserCardsInfoOutput>().FromSqlRaw(sql, sqlParm).ToList();//_repositorySql.ExecuteSqlQuery(sql, sqlParm)?.ToList();
            List<int> lstDoors = new List<int>();
            if (uCardLst?.Count > 0)
            {
                lstDoors = uCardLst.Select(s => s.door_id).Distinct().ToList();
            }
            if (lstDoors?.Count > 0)
            {
                var query = _repositoryDoors.GetAll().Where(s => lstDoors.Contains(s.id));
                lstDoors.ForEach(s =>
                {
                    View_LstUserAllCardsOutput item = new View_LstUserAllCardsOutput();
                    var queryItem = query.FirstOrDefault(p => p.id == s);
                    item.door_id = s;
                    item.door_name = queryItem?.door_name;
                    item.door_img = queryItem?.door_img;
                    item.CardsInfo = uCardLst.Where(p => p.door_id == s)?.ToList();
                    res.Add(item);
                });
            }
            return res;
        }



        public List<View_UserCardsInfoOutput> GetUserDoorCards(int uid, int doorId)
        {
            List<View_UserCardsInfoOutput> res = new List<View_UserCardsInfoOutput>();
            string sql = @"select A.id,A.door_id,[uid]=A.du_id,A.cid,A.ctype,A.card_sttime,A.card_edtime,A.effective_time,A.limit_week_time,A.limit_day_time,A.is_freeze,A.freeze_edtime
                        ,C.card_name,C.card_desc,
                        B.door_img from [dbo].[DoorUsersCards] A
                        left join [dbo].[Doors] B
                        on A.door_id = B.id
                        left join [dbo].[CardTemplate] C
                        on A.cid = C.id
                        where A.cid is not null and A.uid = @uid and A.door_id = @doorId and A.is_delete=0
                        order by A.id desc ;";
            var sqlParm = new SqlParameter[] {
                new SqlParameter("@uid",uid),
                new SqlParameter("@doorId",doorId),
            };
            res = _repository.dbContext.Set<View_UserCardsInfoOutput>().FromSqlRaw(sql, sqlParm).ToList();//_repositorySql.ExecuteSqlQuery(sql, sqlParm)?.ToList();
            return res;
        }

        public View_UserCardsInfoOutput GetUserCardsInfo(int? id)
        {
            string sql = @"select  A.* ,[door_remark]= B.remark,avatar,nick_name
			                from  [dbo].[DoorUsersCards] A
			                left join [dbo].[DoorUsers] B
			                on A.du_id = B.id
			                left join [dbo].[UserInfos] C
			                on B.uid = C.uid
			                where A.id=@id ";
            var sqlParm = new SqlParameter[] {
                new SqlParameter("@id",id),
            };
            return _repository.dbContext.Set<View_UserCardsInfoOutput>().FromSqlRaw(sql, sqlParm).FirstOrDefault(); //_repositorySql.ExecuteSqlQuery(sql, sqlParm)?.FirstOrDefault();
        }

        public bool AddUserCards(DoorUsersCards model)
        {
            _repository.Insert(model);
            var entity = _repositoryDoorUsers.FirstOrDefault(s => s.id == model.du_id);
            if (entity != null && entity.id > 0)
            {
                if (entity.role == Enum_UserRole.Tourist)
                {
                    entity.role = Enum_UserRole.Member;
                    _repositoryDoorUsers.Update(entity);
                }
            }
            return _repositoryDoorUsers.dbContext.SaveChanges() > 0;
        }
        public bool DeleteUserCards(int? id)
        {
            var entity = _repository.FirstOrDefault(s => s.id == id);
            if (entity != null && entity.id > 0)
            {
                entity.is_delete = true;
                _repository.Update(entity);
            }
            return _repository.dbContext.SaveChanges() > 0;
        }
        public bool UpdateUserCardsInfo(DoorUsersCards model)
        {
            var entity = _repository.FirstOrDefault(s => s.id == model.id);
            if (entity != null && entity.id > 0)
            {
                entity.cid = model.cid;
                entity.ctype = model.ctype;
                entity.card_sttime = model.card_sttime;
                entity.card_edtime = model.card_edtime;
                entity.effective_time = model.effective_time;
                entity.limit_week_time = model.limit_week_time;
                entity.limit_day_time = model.limit_day_time;
                entity.freeze_edtime = model.freeze_edtime;
                entity.is_freeze = model.is_freeze;
                _repository.Update(entity);
                return _repository.dbContext.SaveChanges() > 0;
            }
            return false;
        }

        public List<View_Appoint_UsersCardsInfo> GetAppointDoorUserCardsInfo(int? doorId, int? uid)
        {
            string sql = @"select B.card_name,B.card_type, A.* from [dbo].[DoorUsersCards] A
		                    Left join [dbo].[CardTemplate] B
		                    on A.cid = B.id
		                    where A.door_id=@door_id and uid=@uid and is_delete<>1 ;";
            SqlParameter[] SqlParm = new SqlParameter[]
            {
                new SqlParameter("@door_id",doorId),
                new SqlParameter("@uid",uid),
            };
            return _repository.dbContext.Set<View_Appoint_UsersCardsInfo>().FromSqlRaw(sql, SqlParm).ToList();  //_repositoryAppointDoorUserCardsInfoSql.ExecuteSqlQuery(sql, SqlParm).ToList();
        }

        public bool CheckCardsCanUse(int id)
        {
            var entity = _repository.FirstOrDefault(s => s.id == id);
            if (entity == null || entity.id <= 0) return false;
            if (entity.effective_time.HasValue && entity.effective_time <= 0) return false;
            if (entity.card_sttime.HasValue && entity.card_sttime <= DateTime.Now
                && ((entity.card_edtime.HasValue && entity.card_edtime > DateTime.Now) || !entity.card_edtime.HasValue
                && !entity.is_freeze))
                return true;
            return false;
        }

        public bool DeductionUserCards(int id)
        {
            string sql = @"merge into [dbo].[DoorUsersCards] T
                        using( select id= @id ) AS S
                        on T.id = S.id and isnull(T.effective_time,0) >0
                        when matched then
                            update set T.effective_time =  (T.effective_time-1);";
            SqlParameter[] SqlParam = new SqlParameter[] {
                new SqlParameter("@id",id)
            };
            return _repository.ExecuteSqlCommand(sql, SqlParam) >0;
        }

        public bool CheckCardLimitTimes(int uid, int card_id)
        {
            var entity = _repository.FirstOrDefault(s => s.id == card_id);
            if (entity == null || entity.id <= 0) return false;
            if (entity.limit_day_time.HasValue)
            {
                //获取今天预约了几次
                DateTime dt = DateTime.Now.GetDateTimeWithoutTime();
                DateTime edTime = dt.AddDays(1).GetDateTimeWithoutTime();
                int count= _repositoryAppoints.Count(s => s.uid == uid && s.user_card_id == card_id && !s.is_canceled && !s.is_returncard &&  s.create_time >= dt && s.create_time < edTime);
                if (count >= entity.limit_day_time) return false;
            }
            if (entity.limit_week_time.HasValue)
            {
                //获取这周预约了几次
                DateTime dt = DateTime.Now.GetFirstWeekDay();
                DateTime edTime = DateTime.Now.GetLastDayOfWeek();
                int count = _repositoryAppoints.Count(s => s.uid == uid && s.user_card_id == card_id &&!s.is_canceled && !s.is_returncard && s.create_time >= dt && s.create_time < edTime);
                if (count >= entity.limit_week_time) return false;
            }

            return true;
        }

        public bool RebackUserCards(int? uid,int? course_id)
        {
            string sql = @"merge into [dbo].[DoorUsersCards] T
                        using( select id=  ( select user_card_id from [dbo].[DoorUsersAppoints] where uid=@uid and course_id=@course_id ) ) AS S
                        on T.id = S.id and isnull(T.effective_time,0) >0
                        when matched then
                            update set T.effective_time =  (T.effective_time+1);";
            SqlParameter[] SqlParam = new SqlParameter[] {
                new SqlParameter("@uid",uid),
                new SqlParameter("@course_id",course_id),
            };
            return _repository.ExecuteSqlCommand(sql, SqlParam) > 0;
        }

        public int GetCardTempalteId(int id)
        {
            return _repository.FirstOrDefault(s => s.id == id).cid??0;
        }

        public View_InitialUserCardsInfoOutput GetUserLst_SelfAppint(int doorid, int course_id, string nick)
        {
            View_InitialUserCardsInfoOutput return_res = new View_InitialUserCardsInfoOutput();
            string sql = @"select A.id,A.door_id,A.uid,[door_role]=A.role,[door_remark]=A.remark,B.open_id,B.nick_name,B.avatar,B.gender,B.role,B.tel,B.initial,B.real_name,B.birthday 
			from [dbo].[DoorUsers] A
		    left join [dbo].[UserInfos] B
		    on A.uid = B.uid
		    where A.door_id = @doorid 
			and A.uid not in (select uid from  [dbo].[DoorUsersAppoints] where course_id=@course_id  and is_canceled=0 and is_returncard=0)
			and A.role not in (-1,1)
			and (nick_name like @nick or A.remark like @nick) 
			order by initial;";
            var sqlParm = new SqlParameter[] {
                new SqlParameter("@doorid",doorid),
                new SqlParameter("@course_id",course_id),
                new SqlParameter("@nick",$"%{nick}%"),
            };
            var query = _repository.dbContext.Set<View_UserCardsInfoOutput>().FromSqlRaw(sql, sqlParm).ToList();  //_repositorySql.ExecuteSqlQuery(sql, sqlParm)?.ToList();
            List<string> LstLetters = query.Select(s => s.initial)?.Distinct()?.ToList();
            return_res.initials = LstLetters;
            if (LstLetters?.Count > 0)
            {
                LstLetters.ForEach(s =>
                {
                    View_InitialUserCardsInfoItemOutput item = new View_InitialUserCardsInfoItemOutput()
                    {
                        initial = s,
                        uinfos = query.Where(p => p.initial == s)?.ToList()
                    };
                    return_res.uinfos.Add(item);
                });
            }
            return return_res;
        }

        public void ProcessFreezeCard()
        {
            var lst_process= _repository.GetAll().Where(s => !s.is_delete && s.is_freeze && s.freeze_edtime.HasValue).ToList();
            List<DoorUsersCards> lst_cards = new List<DoorUsersCards>();
            if(lst_process!=null && lst_process.Count > 0)
            {
                lst_process.ForEach(s =>
                {
                    if(Convert.ToDateTime(s.freeze_edtime)<= DateTime.Now)
                    {
                        s.freeze_edtime = null;
                        s.is_freeze = false;
                        lst_cards.Add(s);
                    }
                });
                if(lst_cards!=null && lst_cards.Count > 0)
                {
                    foreach (var item in lst_cards)
                    {
                        _repository.Update(item);
                    }
                }
                _repository.dbContext.SaveChanges();
            }
        }
    }
}

