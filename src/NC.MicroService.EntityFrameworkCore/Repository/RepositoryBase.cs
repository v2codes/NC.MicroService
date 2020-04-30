using Microsoft.EntityFrameworkCore;
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
    /// 仓储基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class RepositoryBase<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey>
    {
        private DbContext _dbContext;
        private DbSet<T> _dbSet => _dbContext.Set<T>();

        public RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Insert
        public virtual int Insert(T entity)
        {
            _dbSet.Add(entity);
            return SaveChanges();
        }

        public virtual Task<int> InsertAsync(T entity)
        {
            _dbSet.AddAsync(entity);
            return SaveChangesAsync();
        }

        public virtual int BatchInsert(ICollection<T> entities)
        {
            _dbSet.AddRange(entities);
            return SaveChanges();
        }

        public virtual Task<int> BatchInsertAsync(ICollection<T> entities)
        {
            _dbSet.AddRange(entities);
            return SaveChangesAsync();
        }
        //public virtual int InertBySql(string sql);
        #endregion

        #region Delete
        public virtual int Delete(T entity)
        {
            _dbSet.Remove(entity);
            return SaveChanges();
        }
        public virtual int Delete(TKey key)
        {
            var instance = Activator.CreateInstance<T>();
            instance.Id = key;
            // var entry = _db.Entry(instance);
            // entry.State = EntityState.Deleted;
            return Delete(instance);
        }
        public virtual int Delete(Expression<Func<T, bool>> @where)
        {
            _dbSet.RemoveRange(_dbSet.Where(@where));
            return SaveChanges();
        }
        public virtual Task<int> DeleteAsync(Expression<Func<T, bool>> @where)
        {
            _dbSet.RemoveRange(_dbSet.Where(@where));
            return SaveChangesAsync();
        }
        //int DeleteBySql(string sql);
        #endregion

        #region Update
        public virtual int Update(T entity)
        {
            _dbSet.Update(entity);
            return SaveChanges();
        }
        public virtual Task<int> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return SaveChangesAsync();
        }
        public virtual int BatchUpdate(ICollection<T> entities)
        {
            _dbSet.UpdateRange(entities);
            return SaveChanges();
        }
        public virtual Task<int> BatchUpdateAsync(ICollection<T> entities)
        {
            _dbSet.UpdateRange(entities);
            return SaveChangesAsync();
        }
        //public virtual int BatchUpdate(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp);
        //public virtual Task<int> BatchUpdateAsync(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp);
        //public virtual int Update(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateFactory);
        //public virtual Task<int> UpdateAsync(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateFactory);
        //int UpdateBySql(string sql);
        #endregion

        #region Query
        public virtual int Count(Expression<Func<T, bool>> @where = null)
        {
            _dbSet.Count(@where);
            return SaveChanges();
        }
        public virtual Task<int> CountAsync(Expression<Func<T, bool>> @where = null)
        {
            _dbSet.Count(@where);
            return SaveChangesAsync();
        }
        public virtual bool Exist(Expression<Func<T, bool>> @where = null)
        {
            return _dbSet.Any(@where);
        }
        public virtual Task<bool> ExistAsync(Expression<Func<T, bool>> @where = null)
        {
            return _dbSet.AnyAsync(@where);
        }
        public virtual T Find(TKey key)
        {
            return _dbSet.Find(key);
        }
        public virtual T Find(Expression<Func<T, bool>> @where = null)
        {
            return _dbSet.FirstOrDefault(@where);
        }
        public virtual T Find(TKey key, Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            if (includeFunc == null)
                return Find(key);
            return includeFunc(_dbSet.Where(m => m.Id.Equals(key))).AsNoTracking().FirstOrDefault();
        }
        public virtual ValueTask<T> FindAsync(TKey key)
        {
            return _dbSet.FindAsync(key);
        }
        public virtual Task<T> FindAsync(Expression<Func<T, bool>> @where = null)
        {
            return _dbSet.FirstOrDefaultAsync(@where);
        }
        public virtual IQueryable<T> Query(Expression<Func<T, bool>> @where = null)
        {
            return _dbSet.Where(@where);
        }
        public virtual Task<List<T>> QueryAsync(Expression<Func<T, bool>> @where = null)
        {
            return _dbSet.Where(@where).ToListAsync();
        }
        public virtual IEnumerable<T> QueryByPagination(Expression<Func<T, bool>> @where, int pageSize, int pageIndex,
                                        bool asc = true,
                                        params Func<T, object>[] @orderby)
        {
            var filter = _dbSet.Where(where);
            if (orderby != null)
            {
                foreach (var func in orderby)
                {
                    filter = asc ? filter.OrderBy(func).AsQueryable() : filter.OrderByDescending(func).AsQueryable();
                }
            }
            return filter.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }
        public virtual IQueryable<T> QueryAll()
        {
            return _dbSet;
        }

        //List<T> GetBySql(string sql);
        //List<TView> GetViews<TView>(string sql);
        //List<TView> GetViews<TView>(string viewName, Func<TView, bool> where);
        #endregion

        #region SaveChanges
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
        #endregion

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
