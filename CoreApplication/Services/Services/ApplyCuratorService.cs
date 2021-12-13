using CoreApplication.Services.IServices;
using CoreEntityFramework;
using CoreEntityFramework.Model;
using CoreEntityFramework.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.Services
{
    public class ApplyCuratorService : IApplyCuratorService
    {
        private IRepository<App_DbContext, ApplyCurator> _repository;
        public ApplyCuratorService(IRepository<App_DbContext, ApplyCurator> repository)
        {
            _repository = repository;
        }
        public ApplyCurator CreateApply(ApplyCurator model)
        {
            _repository.Insert(model);
            if (_repository.dbContext.SaveChanges()>0) return model;
            return null;
        }

        public ApplyCurator GetApplyCurator(string open_Id)
        {
            return _repository.GetAll().FirstOrDefault(s => s.open_id == open_Id);
        }

        public bool UpdateBanners(ApplyCurator model)
        {
            var entity = _repository.FirstOrDefault(s => s.open_id == model.open_id);
            entity.enum_status = model.enum_status;
            _repository.Update(entity);
            return _repository.dbContext.SaveChanges() > 0;
        }
    }
}
