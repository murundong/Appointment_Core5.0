using AutoMapper;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using CoreEntityFramework.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.Services
{
    public class BannerService : IBannerService
    {
        private IRepository<App_DbContext, Banners> _repository;
        private IMapper _mapper;
        public BannerService(IRepository<App_DbContext, Banners> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Banners CreateBanners(Banners model)
        {
            _repository.Insert(model);
            if (_repository.dbContext.SaveChanges() > 0) return model;
            return null;
        }

        public bool DeleteBanner(int id)
        {
            _repository.Delete(new Banners() { id = id });
            return _repository.dbContext.SaveChanges() > 0;
        }

        public Banners GetBannerById(int id)
        {
            return _repository.GetAll().FirstOrDefault(s => s.id == id);
        }

        public List<View_BannerOutput> GetBanners()
        {
            var res = _repository.GetAll().Where(s => s.active).OrderBy(s => s.sort);
            return _mapper.Map<List<View_BannerOutput>>(res);
        }

        public Base_PageOutput<List<Banners>> PageBanners(Base_PageInput input)
        {
            Base_PageOutput<List<Banners>> res = new Base_PageOutput<List<Banners>>();
            if (input == null) input = new Base_PageInput();
            var query = _repository.GetAll();
            res.total = query.Count();
            res.data = query.OrderBy(s => s.sort)
                            .Skip((input.page_index - 1) * input.page_size)
                            .Take(input.page_size).ToList();
            return res;
        }

        public bool UpdateBanners(Banners model)
        {
            var entity = _repository.FirstOrDefault(s => s.id == model.id);
            entity.img = model.img;
            entity.img_type = model.img_type;
            entity.url = model.url;
            entity.active = model.active;
            entity.sort = model.sort;
            _repository.Update(entity);
            return _repository.dbContext.SaveChanges() > 0;
        }
    }
}
