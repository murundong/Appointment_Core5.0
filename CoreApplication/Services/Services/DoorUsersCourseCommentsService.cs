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
    public class DoorUsersCourseCommentsService : IDoorUsersCourseCommentsService
    {
        private IRepository<App_DbContext, DoorUsersCourseComments> _repository;

        public DoorUsersCourseCommentsService(IRepository<App_DbContext, DoorUsersCourseComments> repository)
        {
            _repository = repository;
        }
      
        public bool AddJudge(DoorUsersCourseComments input)
        {
            try
            {
                _repository.Insert(input);
                return _repository.dbContext.SaveChanges() > 0;
            }
            catch (Exception  ex)
            {
                
            }
            return false;
        }
    }
}
