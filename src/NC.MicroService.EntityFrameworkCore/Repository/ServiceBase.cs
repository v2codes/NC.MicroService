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
    /// 仓储服务基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class ServiceBase<T, TKey> : IService<T, TKey> where T : class, IEntity<TKey>
    {
        private readonly IRepository<T, TKey> _repository;
        public ServiceBase(IRepository<T, TKey> repository)
        {
            this._repository = repository;
        }

        #region Insert
        public virtual int Insert(T entity)
        {
            return _repository.Insert(entity);
        }
        public virtual Task<int> InsertAsync(T entity)
        {
            return _repository.InsertAsync(entity);
        }
        public virtual int BatchInsert(ICollection<T> entities)
        {
            return _repository.BatchInsert(entities);
        }
        public virtual Task<int> BatchInsertAsync(ICollection<T> entities)
        {
            return _repository.BatchInsertAsync(entities);
        }
        //int InertBySql(string sql);
        #endregion

        #region Delete
        public virtual int Delete(TKey key)
        {
            return _repository.Delete(key);
        }
        public virtual int Delete(Expression<Func<T, bool>> @where)
        {
            return _repository.Delete(@where);
        }
        public virtual Task<int> DeleteAsync(Expression<Func<T, bool>> @where)
        {
            return _repository.DeleteAsync(@where);
        }
        //int DeleteBySql(string sql);
        #endregion

        #region Update
        public virtual int Update(T entity)
        {
            return _repository.Update(entity);
        }
        public virtual Task<int> UpdateAsync(T entity)
        {
            return _repository.UpdateAsync(entity);
        }
        public virtual int BatchUpdate(ICollection<T> entities)
        {
            return _repository.BatchUpdate(entities);
        }
        public virtual Task<int> BatchUpdateAsync(ICollection<T> entities)
        {
            return _repository.BatchUpdateAsync(entities);
        }
        //int BatchUpdate(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp);
        //Task<int> BatchUpdateAsync(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp);
        //int Update(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateFactory);
        //Task<int> UpdateAsync(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateFactory);
        //int UpdateBySql(string sql);
        #endregion

        #region Query
        public virtual int Count(Expression<Func<T, bool>> @where = null)
        {
            return _repository.Count(@where);
        }
        public virtual Task<int> CountAsync(Expression<Func<T, bool>> @where = null)
        {
            return _repository.CountAsync(@where);
        }
        public virtual bool Exists(Expression<Func<T, bool>> @where = null)
        {
            return _repository.Exists(@where);
        }
        public virtual Task<bool> ExistsAsync(Expression<Func<T, bool>> @where = null)
        {
            return _repository.ExistsAsync(@where);
        }
        public virtual T Find(TKey key)
        {
            return _repository.Find(key);
        }
        public virtual T Find(Expression<Func<T, bool>> @where = null)
        {
            return _repository.Find(@where);
        }
        public virtual T Find(TKey key, Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            return _repository.Find(key, includeFunc);
        }
        public virtual async Task<T> FindAsync(TKey key)
        {
            return await _repository.FindAsync(key);
        }
        public virtual Task<T> FindAsync(Expression<Func<T, bool>> @where = null)
        {
            return _repository.FindAsync(@where);
        }
        public virtual List<T> Query(Expression<Func<T, bool>> @where = null)
        {
            return _repository.Query(@where).ToList();
        }
        public virtual Task<List<T>> QueryAsync(Expression<Func<T, bool>> @where = null)
        {
            return _repository.QueryAsync(@where);
        }
        public virtual List<T> QueryByPagination(Expression<Func<T, bool>> @where, int pageSize, int pageIndex,
                                                        bool asc = true,
                                                        params Func<T, object>[] @orderby)
        {
            return _repository.QueryByPagination(@where, pageSize, pageIndex, asc, @orderby).ToList();
        }
        public virtual List<T> QueryAll()
        {
            return _repository.QueryAll().ToList();
        }
        //List<T> GetBySql(string sql);
        //List<TView> GetViews<TView>(string sql);
        //List<TView> GetViews<TView>(string viewName, Func<TView, bool> where);
        #endregion

        public virtual int SaveChanges()
        {
            return _repository.SaveChanges();
        }
        public virtual Task<int> SaveChangesAsync()
        {
            return _repository.SaveChangesAsync();
        }
    }
}
