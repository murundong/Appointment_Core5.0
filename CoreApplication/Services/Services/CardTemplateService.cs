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

namespace CoreApplication.Services.Services
{
    public class CardTemplateService : ICardTemplateService
    {

        private IRepository<App_DbContext, CardTemplate> _repository;
        private IRepository<App_DbContext, Doors> _repositoryDoors;
        private IRepository<App_DbContext, View_PrevCardTemplateOutput> _repositoryPrevTemplate;
        private IMapper _mapper;

        public CardTemplateService(
            IRepository<App_DbContext, CardTemplate> repository,
            IRepository<App_DbContext, Doors> repositoryDoors,
            IRepository<App_DbContext, View_PrevCardTemplateOutput> repositoryPrevTemplate,
            IMapper mapper
            )
        {
            _repository = repository;
            _repositoryDoors = repositoryDoors;
            _repositoryPrevTemplate = repositoryPrevTemplate;
            _mapper = mapper;
        }

        public CardTemplate CreateCardTemplate(CardTemplate model)
        {
            _repository.Insert(model);
            if (_repository.dbContext.SaveChanges() > 0) return model;
            return null;
        }


        public List<View_PrevCardTemplateOutput> GetAllDoorCardsTemplate(int doorId)
        {
            string sql = @"select [door_img]=(select door_img from [dbo].[Doors] where id =[dbo].[CardTemplate].door_id ), * from [dbo].[CardTemplate]
                            where door_id = @doorId";
            var SqlParam = new SqlParameter[] {
                new SqlParameter("@doorId",doorId)
            };
            var res = _repository.dbContext.Set<View_PrevCardTemplateOutput>().FromSqlRaw(sql, SqlParam); //_repositoryPrevTemplate.ExecuteSqlQuery(sql, SqlParam);
            return res.ToList();
        }

        public CardTemplate GetCardsTemplateById(int id)
        {
            return _repository.FirstOrDefault(s => s.id == id);
        }

        public List<ViewDoorCardsSelect> GetDoorCards(int doorId)
        {
            var res = _repository.GetAll().Where(s => s.door_id == doorId);
            return _mapper.Map<List<ViewDoorCardsSelect>>(res);
        }

        public Base_PageOutput<View_CardTemplateOutputItem> PageCardTemplate(View_CardTemplateInput input)
        {
            Base_PageOutput<View_CardTemplateOutputItem> res = new Base_PageOutput<View_CardTemplateOutputItem>() { data = new View_CardTemplateOutputItem() { temps = new List<View_CardTemplateOutput>() } };
            if (input == null || input.door_id <= 0) return res;
            var query = _repository.GetAll()
                .Where(s => s.door_id == input.door_id);
            res.total = query.Count();
            var query_end = query.OrderByDescending(s => s.create_time)
                .Skip((input.page_index - 1) * input.page_size)
                .Take(input.page_size);
            res.data.temps = _mapper.Map<List<View_CardTemplateOutput>>(query_end);
            res.data.img = _repositoryDoors.FirstOrDefault(s => s.id == input.door_id)?.door_img;
            return res;
        }

        public bool UpdateCardTemplate(CardTemplate model)
        {
            var entity = _repository.FirstOrDefault(s => s.id == model.id);
            if (entity == null || entity.id <= 0) return false;

            var time = entity.create_time;
            var oid = entity.create_openid;

            _mapper.Map(model, entity);
            entity.create_time = time;
            entity.create_openid = oid;
            _repository.Update(entity);
            return _repository.dbContext.SaveChanges() > 0;

        }
    }
}
