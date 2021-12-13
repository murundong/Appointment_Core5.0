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

namespace Appoint.Application.Services
{
    public class DoorUsersSubsMsgService : IDoorUsersSubsMsgService
    {
        private IRepository<App_DbContext, DoorUsersSubsMsg> _repository;
        public DoorUsersSubsMsgService(IRepository<App_DbContext, DoorUsersSubsMsg> repository)
        {
            _repository = repository;
        }
     

        public bool AddData(DoorUsersSubsMsg model)
        {
             _repository.Insert(model);
            return _repository.dbContext.SaveChanges() > 0;
        }

        public List<DoorUsersSubsMsg> GetNeedSubs()
        {
            return _repository.GetAll().Where(s => !s.is_cancel || !s.is_notice ).ToList();
        }

        public bool UpdateCancel(int id)
        {
            var entity = _repository.FirstOrDefault(s => s.id == id);
            if(entity!=null && entity.id > 0)
            {
                entity.is_cancel = true;
            }
            return false;
        }

        public bool UpdateNotice(int id)
        {
            var entity = _repository.FirstOrDefault(s => s.id == id);
            if (entity != null && entity.id > 0)
            {
                entity.is_notice = true;
            }
            return false;
        }

        public bool UpdateQueen(int id)
        {
            var entity = _repository.FirstOrDefault(s => s.id == id);
            if (entity != null && entity.id > 0)
            {
                entity.is_queen = true;
            }
            return false;
        }
    }
}
