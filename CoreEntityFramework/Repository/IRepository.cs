using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.Repository
{
    public interface IRepository<TDbContext,TEntity> where TEntity:class
        where TDbContext:DbContext
    {

         TDbContext dbContext { get; }

        #region Select/Get/Query

        IQueryable<TEntity> GetAll();


        List<TEntity> GetAllList();

        //DbSqlQuery<TEntity> SqlQuery(string sql, params object[] parameters);
       
        //DbRawSqlQuery<TEntity> ExecuteSqlQuery(string sql, params object[] parameters);
        int ExecuteSqlCommand(string sql, params object[] parameters);
        Task<List<TEntity>> GetAllListAsync();

        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);


        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);



        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);







        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);


        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);



        #endregion

        #region Insert

        TEntity Insert(TEntity entity);

        void Insert(List<TEntity> entitys);
        Task<TEntity> InsertAsync(TEntity entity);



        #endregion

        #region Update

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);



        #endregion

        #region Delete

        void Delete(TEntity entity);


        Task DeleteAsync(TEntity entity);






        void Delete(Expression<Func<TEntity, bool>> predicate);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregates


        int Count();


        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);


        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);




        #endregion
    }
}
