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
    public class NoticeService : INoticeService
    {
        private IRepository<App_DbContext, Notice> _repository;
        private IRepository<App_DbContext, View_NoticeOutput> _repositoryView;

        public NoticeService(
            IRepository<App_DbContext, Notice> repository,
        IRepository<App_DbContext, View_NoticeOutput> repositoryView
            )
        {
            _repository = repository;
            _repositoryView = repositoryView;
        }

        public Notice CreateNotice(Notice model)
        {
            _repository.Insert(model);
            if (_repository.dbContext.SaveChanges() > 0) return model;
            return null;
        }

        public bool DeleteNotice(int id)
        {
            _repository.Delete(new Notice() { id = id });
            return _repository.dbContext.SaveChanges() > 0;
        }

        public Notice GetNewestNotice()
        {
            return _repository.GetAll().OrderByDescending(s => s.create_time).FirstOrDefault();
        }

        public Base_PageOutput<List<View_NoticeOutput>> GetNotice(View_NoticeInput input)
        {
            Base_PageOutput<List<View_NoticeOutput>> res = new Base_PageOutput<List<View_NoticeOutput>>();
            if (input == null) input = new View_NoticeInput();
            string sql = $@"select [role]=B.role,B.nick_name, A.* from [dbo].[Notice] A
		                    left join  [dbo].[UserInfos] B
		                    on A.uid= B.uid";
            var query = _repository.dbContext.Set<View_NoticeOutput>().FromSqlRaw(sql); //_repositoryView.ExecuteSqlQuery(sql);
            res.total = query.Count();
            var queryPage = query.OrderByDescending(s => s.create_time)
                            .Skip((input.page_index - 1) * input.page_size)
                            .Take(input.page_size);
            res.data = queryPage.ToList();
            return res;
        }

        public Notice GetNoticeItem(int id)
        {
            return _repository.FirstOrDefault(s => s.id == id);
        }

        public bool UpdateNotice(Notice model)
        {
            var entity = _repository.FirstOrDefault(s => s.id == model.id);
            entity.title = model.title;
            entity.msg = model.msg;
            _repository.Update(entity);
            return _repository.dbContext.SaveChanges() > 0;
        }
    }
}
