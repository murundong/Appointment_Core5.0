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
    public class DoorNoticeService : IDoorNoticeService
    {
        private IRepository<App_DbContext, DoorNotice> _repository;
        private IRepository<App_DbContext, View_DoorNoticeOutput> _repositoryView;
        private IRepository<App_DbContext, View_UserNoticeOutput> _repositoryUserNotice;
        public DoorNoticeService(
            IRepository<App_DbContext, DoorNotice> repository,
        IRepository<App_DbContext, View_DoorNoticeOutput> repositoryView,
        IRepository<App_DbContext, View_UserNoticeOutput> repositoryUserNotice
            )
        {
            _repository = repository;
            _repositoryView = repositoryView;
            _repositoryUserNotice = repositoryUserNotice;
        }

        public DoorNotice CreateDoorNotice(DoorNotice model)
        {
            _repository.Insert(model);
            if (_repository.dbContext.SaveChanges() > 0) return model;
            return null;
        }

        public bool DeleteDoorNotice(int id)
        {
            _repository.Delete(new DoorNotice() { id = id });
            return _repository.dbContext.SaveChanges() > 0;
        }

        public Base_PageOutput<List<View_DoorNoticeOutput>> GetDoorNotice(View_DoorNoticeInput input)
        {
            Base_PageOutput<List<View_DoorNoticeOutput>> res = new Base_PageOutput<List<View_DoorNoticeOutput>>();
            if (input == null) input = new View_DoorNoticeInput();
            string sql = $@"select [door_role]=B.role,[nick_name]=C.nick_name, A.* from  [dbo].[DoorNotice]	A
		    left join [dbo].[DoorUsers] B
		    on A.du_id = B.id
		    left join  [dbo].[UserInfos] C
		    on B.uid = C.uid  where A.door_id='{input.door_id}'";
            var query = _repository.dbContext.Set<View_DoorNoticeOutput>().FromSqlRaw(sql); //_repositoryView.ExecuteSqlQuery(sql);
            res.total = query.Count();
            var queryPage = query.OrderByDescending(s=>s.create_time)
                            .Skip((input.page_index - 1) * input.page_size)
                            .Take(input.page_size);
            res.data = queryPage.ToList();
            return res;
        }

        public DoorNotice GetDoorNoticeItem(int id)
        {
            return _repository.FirstOrDefault(s => s.id == id);
        }

        public DoorNotice GetDoorNewestNotice(int door_id)
        {
            return _repository.GetAll().Where(s=>s.door_id == door_id && s.active).OrderByDescending(s => s.create_time).FirstOrDefault();
        }

        public bool UpdateDoorNotice(DoorNotice model)
        {
            var entity = _repository.FirstOrDefault(s => s.id == model.id);
            entity.title = model.title;
            entity.msg = model.msg;
            entity.active = model.active;
            _repository.Update(entity);
            return _repository.dbContext.SaveChanges() > 0;
        }

        public Base_PageOutput<List<View_UserNoticeOutput>> GetUserNotice(View_UserNoticeInput input)
        {
            Base_PageOutput<List<View_UserNoticeOutput>> res = new Base_PageOutput<List<View_UserNoticeOutput>>();
            string sql =$@"select [is_system]=1, [img]=B.avatar,[nick]=B.nick_name,A.title,A.msg,A.create_time from  [dbo].[Notice] A
		                        left join [dbo].[UserInfos] B on A.uid = B.uid
                        union all
                        select [is_system]=0, [img]=C.avatar,[nick]=C.nick_name,A.title,A.msg,A.create_time  from  [dbo].[DoorNotice] A
		                        left join [dbo].[DoorUsers] B on A.du_id = B.id
		                        left join [dbo].[UserInfos] C on B.uid = C.uid
		                        where A.door_id in(select distinct(door_id) from  [dbo].[DoorUsers] where uid = {input.uid})";
            var query = _repository.dbContext.Set<View_UserNoticeOutput>().FromSqlRaw(sql); //_repositoryUserNotice.ExecuteSqlQuery(sql);
            res.total = query.Count();
            var queryPage = query.OrderByDescending(s=>s.is_system).ThenByDescending(s => s.create_time)
                           .Skip((input.page_index - 1) * input.page_size)
                           .Take(input.page_size);
            res.data = queryPage.ToList();
            return res; 
        }
    }
}
