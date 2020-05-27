using NC.MicroService.EntityFrameworkCore.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NC.MicroService.EntityFrameworkCore.Repository
{
    /// <summary>
    /// 仓储服务抽象接口
    /// </summary>
    public interface IService<T, TKey> where T : IEntity<TKey>
    {
        #region Insert
        int Insert(T entity);
        Task<int> InsertAsync(T entity);
        int BatchInsert(ICollection<T> entities);
        Task<int> BatchInsertAsync(ICollection<T> entities);
        //int InertBySql(string sql);
        #endregion

        #region Delete
        int Delete(T entity);
        int Delete(TKey key);
        int Delete(Expression<Func<T, bool>> @where);
        Task<int> DeleteAsync(Expression<Func<T, bool>> @where);
        //int DeleteBySql(string sql);
        #endregion

        #region Update
        int Update(T entity);
        Task<int> UpdateAsync(T entity);
        int BatchUpdate(ICollection<T> entities);
        Task<int> BatchUpdateAsync(ICollection<T> entities);
        //int BatchUpdate(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp);
        //Task<int> BatchUpdateAsync(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp);
        //int Update(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateFactory);
        //Task<int> UpdateAsync(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateFactory);
        //int UpdateBySql(string sql);
        #endregion

        #region Query
        int Count(Expression<Func<T, bool>> @where = null);
        Task<int> CountAsync(Expression<Func<T, bool>> @where = null);
        bool Exists(Expression<Func<T, bool>> @where = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> @where = null);
        T Find(TKey key);
        T Find(Expression<Func<T, bool>> @where = null);
        T Find(TKey key, Func<IQueryable<T>, IQueryable<T>> includeFunc);
        Task<T> FindAsync(TKey key);
        Task<T> FindAsync(Expression<Func<T, bool>> @where = null);
        List<T> Query(Expression<Func<T, bool>> @where = null);
        Task<List<T>> QueryAsync(Expression<Func<T, bool>> @where = null);
        List<T> QueryByPagination(Expression<Func<T, bool>> @where, int pageSize, int pageIndex,
                                        bool asc = true,
                                        params Func<T, object>[] @orderby);
        List<T> QueryAll();
        //List<T> GetBySql(string sql);
        //List<TView> GetViews<TView>(string sql);
        //List<TView> GetViews<TView>(string viewName, Func<TView, bool> where);
        #endregion

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
