using AutoMapper;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
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

namespace Appoint.Application.Services
{
    public class CoursesService : ICoursesService
    {

        private IRepository<App_DbContext, Courses> _repository;
        private IRepository<App_DbContext, View_ServiceCourseModel> _repositoryServiceCourse;
        private IRepository<App_DbContext, Subjects> _repositorySubject;
        private IRepository<App_DbContext, CardTemplate> _repositoryCards;
        private IRepository<App_DbContext, View_CourseShowOutput_AppointUser> _repositoryAppointUser;
        private IRepository<App_DbContext, View_JudgeCourseOutput> _repositoryJudgeCourse;
        private IMapper _mapper;

        public CoursesService(
            IRepository<App_DbContext, Courses> repository,
            IRepository<App_DbContext, View_ServiceCourseModel> repositoryServiceCourse,
            IRepository<App_DbContext, Subjects> repositorySubject,
            IRepository<App_DbContext, CardTemplate> repositoryCards,
            IRepository<App_DbContext, View_CourseShowOutput_AppointUser> repositoryAppointUser,
            IRepository<App_DbContext, View_JudgeCourseOutput> repositoryJudgeCourse,
            IMapper mapper
            )
        {
            _repository = repository;
            _repositoryServiceCourse = repositoryServiceCourse;
            _repositorySubject = repositorySubject;
            _repositoryCards = repositoryCards;
            _repositoryAppointUser = repositoryAppointUser;
            _repositoryJudgeCourse = repositoryJudgeCourse;
            _mapper = mapper;
        }

        public bool CheckCourseCanCancel(int courseid)
        {
            try
            {
                var entity = _repository.FirstOrDefault(s => s.id == courseid);
                if (entity == null || entity.id <= 0) return false;
                var entitySubject = _repositorySubject.FirstOrDefault(s => s.id == entity.subject_id);
                if (entitySubject == null || entitySubject.id <= 0) return false;
                if (entity.cancel_duration.HasValue)
                {
                    DateTime sttime;
                    if (!DateTime.TryParse($"{entity.course_date} {entity.course_time}", out sttime)) return false;
                    if (DateTime.Now >= sttime.AddMinutes(Convert.ToDouble((entity.cancel_duration * -1)))) return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }

        public bool CheckCourseNeedCards(int course_id)
        {
            var entity= _repository.FirstOrDefault(s => s.id == course_id);
            if (entity == null || entity.id <= 0) return false;
            return !string.IsNullOrWhiteSpace(entity.need_cards);
        }

        public Courses CreateCourse(Courses model)
        {
            _repository.Insert(model);
            if (_repository.dbContext.SaveChanges() > 0) return model;
            return null;
        }

        public bool DeleteCourse(int cid)
        {
            _repository.Delete(new Courses() { id = cid });
            return _repository.dbContext.SaveChanges() > 0;
        }

        public List<View_ServiceCourseModel> GetAllCourse(string cids)
        {
            List<View_ServiceCourseModel> res = new List<View_ServiceCourseModel>();
            string sql = $@"select id, door_id,[door_name]=(select door_name from [dbo].[Doors] where id=  [dbo].[Courses].door_id), course_date,course_time,subject_id,cancel_duration,[subject_title]=(select subject_name from [dbo].[Subjects] where id =[dbo].[Courses].subject_id ) 
                           from [dbo].[Courses] where id in ({cids})";
            res = _repository.dbContext.Set<View_ServiceCourseModel>().FromSqlRaw(sql).ToList(); //_repositoryServiceCourse.ExecuteSqlQuery(sql).ToList();
            return res;
        }

        public List<View_ServiceCourseModel> GetAllCourse()
        {
            List<View_ServiceCourseModel> res = new List<View_ServiceCourseModel>();
            string sql = @"select appoint_id=A.id,A.uid,D.open_id,
                    B.id,B.door_id,E.door_name,B.course_date,B.course_time,B.subject_id,[subject_title]=C.subject_name,B.cancel_duration from [dbo].[DoorUsersAppoints] A
			                    left join [dbo].[Courses] B on A.course_id = B.id
			                    left join [dbo].[Subjects] C on B.subject_id = C.id	
			                    left join [dbo].[UserInfos] D on A.uid = D.uid
			                    left join [dbo].[Doors] E on B.door_id = E.id
                    where A.is_canceled=0 and A.is_returncard = 0 and A.is_subsmsg=0 and B.active=1 
                    and  (dateadd(mi,cancel_duration*-1-20,CONVERT(datetime,course_date +' '+course_time) )) <= GETDATE();";
            //res = _repositoryServiceCourse.ExecuteSqlQuery(sql).ToList();
            res= _repository.dbContext.Set<View_ServiceCourseModel>().FromSqlRaw(sql).ToList();
            return res;
        }

        public List<View_ServiceCourseModel> GetAllCourse(int cid)
        {
            List<View_ServiceCourseModel> res = new List<View_ServiceCourseModel>();
            string sql = $@"select appoint_id=A.id,A.uid,D.open_id,
                    B.id,B.door_id,E.door_name,B.course_date,B.course_time,B.subject_id,[subject_title]=C.subject_name,B.cancel_duration from [dbo].[DoorUsersAppoints] A
			                    left join [dbo].[Courses] B on A.course_id = B.id
			                    left join [dbo].[Subjects] C on B.subject_id = C.id	
			                    left join [dbo].[UserInfos] D on A.uid = D.uid
			                    left join [dbo].[Doors] E on B.door_id = E.id
                    where B.id={cid} ;";
            res = _repository.dbContext.Set<View_ServiceCourseModel>().FromSqlRaw(sql).ToList(); //_repositoryServiceCourse.ExecuteSqlQuery(sql).ToList();
            return res;
        }

        public Courses GetCourseById(int id)
        {
            return _repository.FirstOrDefault(s => s.id == id);
        }

        public List<int> GetCourseCards(int courseid)
        {
            var query = _repository.FirstOrDefault(s => s.id == courseid);
            if (query == null || string.IsNullOrWhiteSpace(query.need_cards)) return null;
            return query.need_cards.Split(',')?.Select<string,int>(s=>Convert.ToInt32(s))?.ToList();
        }

        public Base_PageOutput<List<View_CoursesOutput>> GetCourses(View_CoursesInput input)
        {
            Base_PageOutput<List<View_CoursesOutput>> res = new Base_PageOutput<List<View_CoursesOutput>>();

            var query = _repository.GetAll().Where(s => s.door_id == input.door_id);
            if (!string.IsNullOrWhiteSpace(input.date))
            {
                query = query.Where(s => s.course_date == input.date);
            }
            res.total = query.Count();
            var query_end = query.OrderBy(s => s.course_time)
                .Skip((input.page_index - 1) * input.page_size)
                .Take(input.page_size);
            var lst = _mapper.Map<List<View_CoursesOutput>>(query_end);
            if (lst != null && lst.Count > 0)
            {
                //已预约名单
                string sql = $@"select  A.id,du_id,course_id,A.uid,avatar,[door_remark]= B.remark,nick_name from  [dbo].[DoorUsersAppoints] A
			                        Left join [dbo].[DoorUsers] B
			                        on A.du_id = B.id
			                        left join  [dbo].[UserInfos] C
			                        on A.uid = C.uid
			                        where course_id in ({string.Join(",", lst.Select(s => s.id))}) and is_canceled = 0 and is_returncard=0 order by A.create_time;";
                var queryAppointUser = _repository.dbContext.Set<View_CourseShowOutput_AppointUser>().FromSqlRaw(sql).ToList();// _repositoryAppointUser.ExecuteSqlQuery(sql)?.ToList();
                //排队名单
                string sqlQueue = $@"select  A.id,du_id,course_id,A.uid,avatar,[door_remark]= B.remark,nick_name from  [dbo].[DoorUsersQueueAppoints]  A
			                        Left join [dbo].[DoorUsers] B
			                        on A.du_id = B.id
			                        left join  [dbo].[UserInfos] C
			                        on A.uid = C.uid
			                        where course_id in ({string.Join(",", lst.Select(s => s.id))}) order by A.create_time;";
                var queryQueueAppointUser = _repository.dbContext.Set<View_CourseShowOutput_AppointUser>().FromSqlRaw(sqlQueue).ToList(); //_repositoryAppointUser.ExecuteSqlQuery(sqlQueue)?.ToList();
                lst.ForEach(s =>
                {
                    var sub_item = _repositorySubject.FirstOrDefault(p => p.id == s.subject_id);
                    if (sub_item != null && sub_item.id > 0)
                    {
                        s.Subject = _mapper.Map<View_SubjectsOutput>(sub_item);
                    }
                    s.AppointUsers = queryAppointUser?.Where(p => p.course_id == s.id)?.ToList();
                    if (s.allow_queue)
                    {
                        s.QueueAppointUsers = queryQueueAppointUser?.Where(p => p.course_id == s.id)?.ToList();
                    }
                });
            }
            res.data = lst;
            return res;
        }

        public Base_PageOutput<List<View_CourseShowOutput>> GetDoorAppointCourse(View_GetAppointCourseInput input)
        {
            Base_PageOutput<List<View_CourseShowOutput>> return_res = new Base_PageOutput<List<View_CourseShowOutput>>();
            var query =  _repository.GetAll().Where(s => s.door_id == input.doorId && s.course_date == input.date );
            var querySubject = _repositorySubject.GetAll().Where(s => s.door_id == input. doorId);
            var lstCourse = _mapper.Map<List<View_CourseShowOutput>>(query.ToList());
            if (lstCourse?.Count > 0)
            {
                lstCourse.ForEach(s =>
                {
                    var subject_item = querySubject.FirstOrDefault(p => p.id == s.subject_id);
                    s.Subject = _mapper.Map<View_SubjectsOutput>(subject_item);
                    if (!string.IsNullOrWhiteSpace(s.need_cards))
                    {
                        var temp_cards = s.need_cards.Split(',');
                        s.NeedCardNames = _repositoryCards.GetAll().Where(p => temp_cards.Contains(p.id.ToString())).Select(p => p.card_name).ToList();
                    }
                });
                if (!string.IsNullOrWhiteSpace(input.tag))
                {
                    lstCourse = lstCourse?.Where(s => s.Subject.subject_tag?.Contains(input.tag)==true)?.ToList();
                }
                return_res.total = lstCourse.Count;
                return_res.data= lstCourse.OrderBy(s=>s.course_time)
                     .Skip((input.page_index - 1) * input.page_size)
                     .Take(input.page_size)?.ToList();
                if (return_res.data?.Count > 0)
                {
                    //已预约名单
                    string sql = $@"select  A.id,du_id,course_id,A.uid,avatar,[door_remark]= B.remark,nick_name from  [dbo].[DoorUsersAppoints] A
			                        Left join [dbo].[DoorUsers] B
			                        on A.du_id = B.id
			                        left join  [dbo].[UserInfos] C
			                        on A.uid = C.uid
			                        where course_id in ({string.Join(",", return_res.data.Select(s=>s.id))}) and is_canceled = 0 and is_returncard = 0 order by A.create_time;";
                    var queryAppointUser = _repository.dbContext.Set<View_CourseShowOutput_AppointUser>().FromSqlRaw(sql).ToList(); //_repositoryAppointUser.ExecuteSqlQuery(sql)?.ToList();

                    //排队名单
                    string sqlQueue = $@"select  A.id,du_id,course_id,A.uid,avatar,[door_remark]= B.remark,nick_name from  [dbo].[DoorUsersQueueAppoints]  A
			                        Left join [dbo].[DoorUsers] B
			                        on A.du_id = B.id
			                        left join  [dbo].[UserInfos] C
			                        on A.uid = C.uid
			                        where course_id in ({string.Join(",", return_res.data.Select(s => s.id))}) order by A.create_time;";
                    var queryQueueAppointUser = _repository.dbContext.Set<View_CourseShowOutput_AppointUser>().FromSqlRaw(sqlQueue).ToList();//_repositoryAppointUser.ExecuteSqlQuery(sqlQueue)?.ToList();
                    return_res.data.ForEach(s =>
                    {
                        s.AppointUsers = queryAppointUser?.Where(p => p.course_id == s.id)?.ToList();
                        if (s.allow_queue)
                        {
                            s.QueueAppointUsers = queryQueueAppointUser?.Where(p => p.course_id == s.id)?.ToList();
                        }
                    });

                }
            }
            return return_res;
        }

        public View_JudgeCourseOutput GetJudgeCourseInfo(int? cid)
        {
            string sql = $@"select B.subject_name,subject_teacher,subject_img,subject_duration, A.* from [dbo].[Courses] A
	                        left join [dbo].[Subjects] B
	                        on A.subject_id = B.id
	                        where A.id={cid} ;";
            return _repository.dbContext.Set<View_JudgeCourseOutput>().FromSqlRaw(sql).FirstOrDefault();//_repositoryJudgeCourse.ExecuteSqlQuery(sql).FirstOrDefault();
        }

        public View_CoursesOutput GetSignCourseById(int course_Id)
        {
            View_CoursesOutput res = new View_CoursesOutput();
            var query = _repository.FirstOrDefault(s => s.id == course_Id);
            res = _mapper.Map<View_CoursesOutput>(query);
            if(res!=null)
            {
                var query_subject = _repositorySubject.FirstOrDefault(p => p.id == res.subject_id);
                res.Subject = _mapper.Map<View_SubjectsOutput>(query_subject);
                string sql = $@"select  A.id,du_id,course_id,A.uid,avatar,[door_remark]= B.remark,nick_name from  [dbo].[DoorUsersAppoints] A
			                        Left join [dbo].[DoorUsers] B
			                        on A.du_id = B.id
			                        left join  [dbo].[UserInfos] C
			                        on A.uid = C.uid
			                        where course_id ={course_Id} and is_canceled = 0 and is_returncard=0 order by A.create_time;";
                res.AppointUsers = _repository.dbContext.Set<View_CourseShowOutput_AppointUser>().FromSqlRaw(sql).ToList();// _repositoryAppointUser.ExecuteSqlQuery(sql).ToList();
            }
            return res;
        }

        public List<View_WeekCourseOutput> GetWeekCourse(View_WeekCourseInput input)
        {
            DateTime st_dt , ed_dt ;
            DateTime dt = DateTime.Now;
            
            if(input.tp == -1)
            {
                st_dt = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")) - 7);
                ed_dt = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")) + 6 - 7);
            }
            else if (input.tp == 1)
            {
                st_dt = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")) + 7);
                ed_dt = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")) + 6 + 7);
            }
            else
            {
                st_dt = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")));
                ed_dt = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")) + 6);
            }
            List<View_WeekCourseOutput> res = new List<View_WeekCourseOutput>();
            string[] Day = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string sql = $"select * from [dbo].[Courses] where door_id={input.door_id} and course_date >='{st_dt.ToString("yyyy-MM-dd")}'  and course_date <= '{ed_dt.ToString("yyyy-MM-dd")}' order by course_date,course_time";
            //var query = _repository.SqlQuery($"select * from [dbo].[Courses] where door_id={input.door_id} and course_date >='{st_dt.ToString("yyyy-MM-dd")}'  and course_date <= '{ed_dt.ToString("yyyy-MM-dd")}' order by course_date,course_time");
            var query = _repository.dbContext.Set<Courses>().FromSqlRaw(sql);

            for (int i = 0; i < 7; i++)
            {
                View_WeekCourseOutput itemModel = new View_WeekCourseOutput()
                {

                    date = st_dt.AddDays(i).ToString("yyyy-MM-dd"),
                    week = Day[(int)st_dt.AddDays(i).DayOfWeek],
                    Courses = _mapper.Map<List<View_CoursesOutput>>(query?.Where(s => s.course_date == st_dt.AddDays(i).ToString("yyyy-MM-dd")))
                };
                if (itemModel.Courses?.Count > 0)
                {
                    itemModel.Courses.ForEach(s =>
                    {
                        s.Subject = _mapper.Map<View_SubjectsOutput>(_repositorySubject.FirstOrDefault(p => p.id == s.subject_id));
                    });
                }
                res.Add(itemModel);
            }
            return res;

        }

     

        public bool QuickCourse(string sdate, string cdate, int doorid, string openid)
        {
            List<Courses> insertLst = new List<Courses>();
            var lstCourse = _repository.GetAll().Where(s => s.course_date == sdate && s.create_openid == openid && s.door_id == doorid);
            if (lstCourse.Count() <= 0)
            {
                return false;
            }
            lstCourse.ToList().ForEach(s =>
            {
                Courses itemCourse = new Courses();
                _mapper.Map(s, itemCourse);
                itemCourse.course_date = cdate;
                itemCourse.temp_teacher = string.Empty;
                itemCourse.create_time = DateTime.Now;
                itemCourse.active = true;
                insertLst.Add(itemCourse);
            });
            _repository.Insert(insertLst);
            return _repository.dbContext.SaveChanges() > 0;
        }

        public bool UpdateCourse(Courses model)
        {
            var entity = _repository.FirstOrDefault(s => s.id == model.id);
            var oid = entity.create_openid;
            var time = entity.create_time;
            _mapper.Map(model, entity);
            entity.create_time = time;
            entity.create_openid = oid;
            _repository.Update(entity);
            return _repository.dbContext.SaveChanges() > 0;
        }


    }
}
