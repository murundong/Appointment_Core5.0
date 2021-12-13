using AutoMapper;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
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
    public class DoorUsersQueueAppointsService : IDoorUsersQueueAppointsService
    {
        private IRepository<App_DbContext, DoorUsersQueueAppoints> _repository;
        private IRepository<App_DbContext, View_ServiceCourseModel> _repositoryServiceCourse;

        public DoorUsersQueueAppointsService(
            IRepository<App_DbContext, DoorUsersQueueAppoints> repository,
        IRepository<App_DbContext, View_ServiceCourseModel> repositoryServiceCourse
            )
        {
            _repository = repository;
            _repositoryServiceCourse = repositoryServiceCourse;
        }
    
        public bool CancelQueue(int uid, int courseid)
        {
            var entity = _repository.FirstOrDefault(s => s.uid == uid && s.course_id == courseid);
            if (entity == null || entity.id <= 0) return false;
            _repository.Delete(entity);
            return _repository.dbContext.SaveChanges() > 0;
        }

        public bool CheckUserAlreadyQuee(int uid, int courseid)
        {
            return _repository.Count(s => s.uid == uid && s.course_id == courseid) > 0;
        }

        public bool DeleteUserQueue(DoorUsersQueueAppoints entity)
        {
            _repository.Delete(entity);
            return _repository.dbContext.SaveChanges()> 0;
        }

        public View_ServiceCourseModel GetQueenNoticDetail(int uid, int cid)
        {
            string sql = $@"select A.id, [open_id]=(select open_id from [dbo].[UserInfos] where uid={uid}) ,
		B.door_id,door_name,course_date,course_time,subject_id,[subject_title]=subject_name,cancel_duration
		from [dbo].[Courses]	A
		left join [dbo].[Subjects] B on A.subject_id = B.id	
		left join [dbo].[Doors] C on B.door_id = C.id
		where A.id={cid};";
            return _repository.dbContext.Set<View_ServiceCourseModel>().FromSqlRaw(sql).FirstOrDefault(); //_repositoryServiceCourse.ExecuteSqlQuery(sql)?.FirstOrDefault();
        }

        public DoorUsersQueueAppoints GetQueueUser(int courseid)
        {
            var entity = _repository.GetAll().Where(s => s.course_id == courseid).OrderBy(s => s.create_time).FirstOrDefault();
            return entity;
        }

        public bool Queue(View_AppointCourseInput input)
        {
            if (input.uid <= 0 || input.course_id <= 0 || input.door_id <= 0) return false;
            string sql = @"merge into [dbo].[DoorUsersQueueAppoints] T
                    using ( select du_id=(select top 1 id from [dbo].[DoorUsers] where door_id=@door_id and uid=@uid)
                    ,uid=@uid
                    ,course_id=@course_id
                    ,user_card_id=@user_card_id
                    ,create_time=getdate()) S
                    on T.uid = S.uid and T.course_id= S.course_id
                    when not matched then 
                    insert ([du_id]
                    ,[uid]
                    ,[course_id]
                    ,[user_card_id]
                    ,[create_time])
                    values( S.[du_id]
                    ,S.[uid]
                    ,S.[course_id]
                    ,S.[user_card_id]
                    ,S.[create_time]);";
            try
            {
                SqlParameter[] SqlParm = new SqlParameter[]
                {
                    new SqlParameter("@door_id",input.door_id),
                    new SqlParameter("@uid",input.uid),
                    new SqlParameter("@course_id",input.course_id),
                    new SqlParameter("@user_card_id",input.card_id==null?string.Empty:input.card_id?.ToString()),
                };
                return _repository.ExecuteSqlCommand(sql, SqlParm) > 0;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


    }
}
