using AutoMapper;
using CoreApplication.Services.IServices;
using CoreBaseClass;
using CoreEntityFramework;
using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using CoreEntityFramework.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appoint.Application.Services.Services
{
    public class DoorUsersAppointsService : IDoorUsersAppointsService
    {
        private IRepository<App_DbContext, DoorUsersAppoints> _repository;
        private IRepository<App_DbContext, DoorUsers> _repositoryDoorUsers;
        private IRepository<App_DbContext, UserInfos> _repositoryUserInfos;
        private IRepository<App_DbContext, View_WinServiceCourseModel> _repositoryWinservice;
        private IRepository<App_DbContext, View_MyAppointWaitOutput> _repositoryMyWaitAppoint;
        private IRepository<App_DbContext, View_MyAppointCompOutput> _repositoryMyCompAppoint;
        private IRepository<App_DbContext, View_MyAppointCompOutput_Detail> _repositoryMyCompDetailAppoint;
        private IRepository<App_DbContext, Courses> _repositoryCourse;

        public DoorUsersAppointsService(
            IRepository<App_DbContext, DoorUsersAppoints> repository,
            IRepository<App_DbContext, DoorUsers> repositoryDoorUsers,
            IRepository<App_DbContext, UserInfos> repositoryUserInfos,
            IRepository<App_DbContext, View_WinServiceCourseModel> repositoryWinservice,
            IRepository<App_DbContext, View_MyAppointWaitOutput> repositoryMyWaitAppoint,
            IRepository<App_DbContext, View_MyAppointCompOutput> repositoryMyCompAppoint,
            IRepository<App_DbContext, View_MyAppointCompOutput_Detail> repositoryMyCompDetailAppoint,
            IRepository<App_DbContext, Courses> repositoryCourse
            )
        {
            _repository = repository;
            _repositoryDoorUsers = repositoryDoorUsers;
            _repositoryUserInfos = repositoryUserInfos;
            _repositoryWinservice = repositoryWinservice;
            _repositoryMyWaitAppoint = repositoryMyWaitAppoint;
            _repositoryMyCompAppoint = repositoryMyCompAppoint;
            _repositoryMyCompDetailAppoint = repositoryMyCompDetailAppoint;
            _repositoryCourse = repositoryCourse;
        }

        public bool CheckUserExsitBindding(int door_id,int uid)
        {
            string sql = @"select * from [dbo].[DoorUsers] with(nolock) where door_id=@door_id and uid=@uid";
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@door_id",door_id),
                    new SqlParameter("@uid",uid),
                };
                var res = _repository.dbContext.Set<DoorUsers>().FromSqlRaw(sql, sqlParam); //_repositoryDoorUsers.ExecuteSqlQuery(sql, sqlParam);
                return res.Count() > 0;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }


        public bool AddUserAppoint(int uid, int doorid, int courseid, int? cardid)
        {
            string sql = @"merge into [dbo].[DoorUsersAppoints] T
                            using ( select du_id=(select top 1 id from [dbo].[DoorUsers] where door_id=@door_id and uid=@uid)
                                    ,uid=@uid
                                    ,course_id=@course_id
                                    ,user_card_id=@user_card_id
                                    ,create_time=getdate()) S
                            on T.uid = S.uid and T.course_id= S.course_id
                            when matched then 
	                            update set T.is_signed=0,T.signed_time=null,T.is_canceled=0,T.is_returncard=0,T.is_subsmsg=0,T.create_time=S.create_time
                            when not matched then 
	                            insert ([du_id]
                                       ,[uid]
                                       ,[course_id]
                                       ,[user_card_id]
                                       ,[is_signed]
                                       ,[is_canceled]
                                       ,[create_time])
	                            values( S.[du_id]
                                       ,S.[uid]
                                       ,S.[course_id]
                                       ,S.[user_card_id]
                                       ,0
                                       ,0
                                       ,S.[create_time]);";
            try
            {
                SqlParameter[] SqlParm = new SqlParameter[] {
                new SqlParameter("@door_id",doorid),
                new SqlParameter("@uid",uid),
                new SqlParameter("@course_id",courseid),
                new SqlParameter("@user_card_id",(cardid==null?string.Empty:cardid?.ToString())),
                };
                return _repository.ExecuteSqlCommand(sql, SqlParm) > 0;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return false;
        }

        public bool CancelAppoint(int? uid, int? courseid)
        {
            var entity = _repository.FirstOrDefault(s => s.uid == uid && s.course_id == courseid);
            if (entity != null && entity.id > 0)
            {
                entity.is_canceled = true;
                _repository.Update(entity);
                return _repository.dbContext.SaveChanges() > 0;
            }
            return false;
        }
        public bool ReturnAppoint(int? uid, int? courseid)
        {
            var entity = _repository.FirstOrDefault(s => s.uid == uid && s.course_id == courseid);
            if (entity != null && entity.id > 0)
            {
                entity.is_returncard = true;
                _repository.Update(entity);
                return _repository.dbContext.SaveChanges() > 0;
            }
            return false;
        }

        public bool CopyQueueAppoint(DoorUsersQueueAppoints model)
        {

            var entity = _repository.FirstOrDefault(s => s.uid == model.uid && s.course_id == model.course_id);
            if(entity!=null && entity.id > 0)
            {
                entity.user_card_id = model.user_card_id;
                entity.is_canceled = false;
                entity.is_returncard = false;
                entity.is_subsmsg = false;
                _repository.Update(entity);
            }
            else
            {
                DoorUsersAppoints entityInsert = new DoorUsersAppoints()
                {
                    course_id = model.course_id,
                    du_id = model.du_id,
                    uid = model.uid,
                    user_card_id = model.user_card_id,
                };
                _repository.Insert(entityInsert);
            }
            return _repository.dbContext.SaveChanges() > 0;
        }

        public int GetCourseAppointCount(int cid)
        {
            return _repository.Count(s => s.course_id == cid && !s.is_canceled && !s.is_returncard);
        }

        public Enum_AppointStatus GetCourseAppointStatus(int uid, int cid)
        {
            if (uid <= 0) return Enum_AppointStatus.SHOW_NULL;
            if (_repository.Count(s => s.uid == uid && s.course_id == cid && !s.is_canceled && !s.is_returncard) > 0) return Enum_AppointStatus.SHOW_CANCEL;
            var CourseItem = _repositoryCourse.FirstOrDefault(s => s.id == cid && s.active);
            if (CourseItem == null || CourseItem.id <= 0) return Enum_AppointStatus.SHOW_NULL;
            //时间
            DateTime sttime;
            if (!DateTime.TryParse($"{CourseItem.course_date} {CourseItem.course_time}", out sttime)) return Enum_AppointStatus.SHOW_NULL;
            if (sttime.AddMinutes((int.Parse(CourseItem.limit_appoint_duration?.ToString() ?? "0") * -1)) <= DateTime.Now) return Enum_AppointStatus.SHOW_TIMEEXPIRED;
            if (!sttime.TheSameDayAs(DateTime.Now) && CourseItem.only_today_appoint) return Enum_AppointStatus.SHOW_ONLYTODY;
            //人数
            int ct = GetCourseAppointCount(cid);
            if (ct >= CourseItem.max_allow && CourseItem.allow_queue) return Enum_AppointStatus.SHOW_QUEUE;
            if (ct >= CourseItem.max_allow) return Enum_AppointStatus.SHOW_FULLPEOPLE;
            return Enum_AppointStatus.SHOW_APPOINT;
        }
        public bool IsUserAlreadyCancel(int? uid, int? courseid)
        {
            return _repository.Count(s => s.uid == uid && s.course_id == courseid && s.is_canceled) > 0;
        }

        public Base_PageOutput<List<View_MyAppointWaitOutput>> GetMyAppointWait(View_MyAppointWaitInput input)
        {
            Base_PageOutput<List<View_MyAppointWaitOutput>> res = new Base_PageOutput<List<View_MyAppointWaitOutput>>();
            try
            {
                string sql = $@"select  
		                [now_user]=(
			                select count(1) from  [dbo].[DoorUsersAppoints] where course_id= A.course_id and is_canceled=0
		                ),
		                A.*,
		                B.door_id,door_name,door_address,
		                D.course_date,D.course_time,D.max_allow,D.min_allow,
		                E.subject_name,E.subject_duration,E.subject_img
		                from  [dbo].[DoorUsersAppoints] A
		                left join  [dbo].[DoorUsers] B
		                on A.du_id = B.id
		                left join [dbo].[Doors] C
		                on B.door_id = C.id
		                left join  [dbo].[Courses] D
		                on A.course_id = D.id
		                left join [dbo].[Subjects] E
		                on D.subject_id = E.id
                where A.uid = {input.uid} and A.is_canceled=0  
                and  dateadd(mi,E.subject_duration,  CONVERT(datetime,D.course_date+' '+D.course_time,20 )) > GETDATE() ;";
                var query = _repository.dbContext.Set<View_MyAppointWaitOutput>().FromSqlRaw(sql);     //_repositoryMyWaitAppoint.ExecuteSqlQuery(sql);
                res.total = query.Count();
                res.data = query.OrderBy(s => s.course_date).OrderBy(s => s.course_time)
                      .Skip((input.page_index - 1) * input.page_size)
                      .Take(input.page_size)?.ToList();
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public Base_PageOutput<List<View_MyAppointCompOutput>> GetMyAppointComp(View_MyAppointWaitInput input)
        {
            Base_PageOutput<List<View_MyAppointCompOutput>> res = new Base_PageOutput<List<View_MyAppointCompOutput>>();
            try
            {
                string sql = $@"select  [judge]= (select count(1) from [dbo].[DoorUsersCourseComments] where uid = A.uid and course_id = A.course_id),
										dt=LEFT(D.course_date,7),
		                                A.*,
		                                B.door_id,door_name,
		                                D.course_date,D.course_time,
		                                E.subject_name,E.subject_duration,E.subject_img
		                                from  [dbo].[DoorUsersAppoints] A
		                                left join  [dbo].[DoorUsers] B
		                                on A.du_id = B.id
		                                left join [dbo].[Doors] C
		                                on B.door_id = C.id
		                                left join  [dbo].[Courses] D
		                                on A.course_id = D.id
		                                left join [dbo].[Subjects] E
		                                on D.subject_id = E.id
                                where A.uid = {input.uid} and A.is_canceled=0 
								and dateadd(mi, E.subject_duration, CONVERT(datetime, D.course_date+' ' + D.course_time,20 )) < GETDATE()";
                var query = _repository.dbContext.Set<View_MyAppointCompOutput_Detail>().FromSqlRaw(sql); //_repositoryMyCompDetailAppoint.ExecuteSqlQuery(sql);
                res.total = query.Count();
                var queryPage = query.OrderByDescending(s => s.course_date).ThenByDescending(s => s.course_time)
                                .Skip((input.page_index - 1) * input.page_size)
                                .Take(input.page_size);
                res.data = queryPage?.GroupBy(s => s.dt)?.Select(s => new View_MyAppointCompOutput() { dt = s.Key, ct = s.Count() })?.ToList();
                if (res.data != null && res.data.Count > 0)
                {
                    string maxdt = Convert.ToDateTime($"{ res.data[0].dt}-01").AddMonths(1).ToString("yyyy-MM-dd");
                    string mindt = $"{ res.data[res.data.Count() - 1].dt}-01";

                    res.data.ForEach(s =>
                    {
                        s.courses = queryPage.Where(p =>
                         Convert.ToDateTime(p.st_time) >= Convert.ToDateTime(s.dt)
                         && Convert.ToDateTime(p.st_time) < (Convert.ToDateTime(s.dt).AddMonths(1) < DateTime.Now ? Convert.ToDateTime(s.dt).AddMonths(1) : DateTime.Now)
                        )?.ToList();
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return res;

        }

        public bool CancselCourse(int course_id)
        {
            string sql = string.Empty;
            //取消排队
            string queueSql = $@";delete [dbo].[DoorUsersQueueAppoints] where course_id={course_id} ;";
            //课程取消
            string cancelCourseSql = $@";update [dbo].[Courses] set active=0 where id={course_id};";
            sql += queueSql;
            sql += cancelCourseSql;

            //得到已经预约的人
            var query = _repository.GetAll().Where(s => s.course_id == course_id && !s.is_canceled && !s.is_returncard).Select(s => s.uid).ToList();
            if (query?.Count() > 0)
            {
                foreach (var item in query)
                {
                    //退卡
                    string rebackSql = $@";merge into [dbo].[DoorUsersCards] T
                        using( select id=  ( select user_card_id from [dbo].[DoorUsersAppoints] where uid={item} and course_id={course_id} ) ) AS S
                        on T.id = S.id and isnull(T.effective_time,0) >0
                        when matched then
                            update set T.effective_time =  (T.effective_time+1);";
                    //取消预约
                    string cancelSql = $@";update [dbo].[DoorUsersAppoints] set is_canceled=1,is_returncard=0  where course_id={course_id} and uid={item} ;";

                    sql += rebackSql;
                    sql += cancelSql;
                }
            }
            return _repository.ExecuteSqlCommand(sql) > 0;
        }

        public void CancselCourse(out Dictionary<int, List<string>> usercids)
        {
            Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();
            usercids = new Dictionary<int, List<string>>();
            StringBuilder sb = new StringBuilder();
            string sql = @";with cte as(
                        select [end_appoint_time]= (dateadd(mi,limit_appoint_duration*-1,CONVERT(datetime,course_date +' '+course_time) )),
	                           [already_appoint] = (select count(1) from  [dbo].[DoorUsersAppoints] where course_id=[dbo].[Courses].id and is_canceled =0 and is_returncard=0 ),
	                           id,min_allow
	                           from  [dbo].[Courses] where active=1
                         )
                         select * from cte where already_appoint < min_allow and GETDATE() >= end_appoint_time ";
            var courseQuery = _repository.dbContext.Set<View_WinServiceCourseModel>().FromSqlRaw(sql).ToList(); //_repositoryWinservice.ExecuteSqlQuery(sql).ToList();
            if (courseQuery?.Count() >0)
            {
                courseQuery.ForEach(s =>
                {
                    sb.Append($";delete [dbo].[DoorUsersQueueAppoints] where course_id={s.id} ;");
                    sb.Append($@";update [dbo].[Courses] set active=0 where id={s.id};");
                    if (s.already_appoint > 0)
                    {
                        var query = _repository.GetAll().Where(p => p.course_id == s.id && !p.is_canceled && !p.is_returncard).Select(p => p.uid).ToList();
                        if (query?.Count() > 0)
                        {
                            foreach (var item in query)
                            {
                                //退卡
                                string rebackSql = $@";merge into [dbo].[DoorUsersCards] T
                                using( select id=  ( select user_card_id from [dbo].[DoorUsersAppoints] where uid={item} and course_id={s.id} ) ) AS S
                                on T.id = S.id and isnull(T.effective_time,0) >0
                                when matched then
                                update set T.effective_time =  (T.effective_time+1);";
                                //取消预约
                                string cancelSql = $@";update [dbo].[DoorUsersAppoints] set is_canceled=1,is_returncard=0  where course_id={s.id} and uid={item} ;";
                                sb.Append(rebackSql);
                                sb.Append(cancelSql);
                                string open_id = _repositoryUserInfos.FirstOrDefault(m => m.uid == item)?.open_id;
                                if (dic.ContainsKey(s.id))
                                {
                                    dic[s.id].Add(open_id);
                                }
                                else
                                {
                                    dic.Add(s.id, new List<string>() { open_id });
                                }
                            }
                        }
                    }
                
                });
                //course_ids = courseQuery.Select(s => {s.id,s. })?.ToList();
                _repository.ExecuteSqlCommand(sb.ToString());
                usercids = dic;
            }
        }

        public View_UserStatisticOutput GetUserStatistic(int uid)
        {
            View_UserStatisticOutput res = new View_UserStatisticOutput();
            var sqlParms = new SqlParameter[]
            {
                new SqlParameter("@uid",uid),
                new SqlParameter("@total_minutes",System.Data.SqlDbType.Int){Direction= System.Data.ParameterDirection.Output },
                new SqlParameter("@total_times",System.Data.SqlDbType.Int){Direction= System.Data.ParameterDirection.Output },
                new SqlParameter("@total_days",System.Data.SqlDbType.Int){Direction= System.Data.ParameterDirection.Output },
            };

            _repository.ExecuteSqlCommand("exec GetUserDataStatistic @uid,@total_minutes out,@total_times out,@total_days out", sqlParms);
            try
            {
                res.total_minutes = (int)(sqlParms[1].Value ?? 0);
                res.total_times = (int)(sqlParms[2].Value ?? 0);
                res.total_days = (int)(sqlParms[3].Value ?? 0);
            }
            catch (Exception ex)
            {
                
            }
            return res;
                
        }

        public bool UpdateNoticeAppoint(string ids)
        {
            string sql = $"update [dbo].[DoorUsersAppoints] set is_subsmsg=1 where id in ({ids})";
            return _repository.ExecuteSqlCommand(sql)>0;
        }







    }
}

