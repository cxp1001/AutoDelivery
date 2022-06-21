using AutoDelivery.Core.Core;
using System.Linq.Expressions;
using AutoDelivery.Core.Extensions;
using Microsoft.EntityFrameworkCore;


namespace AutoDelivery.Core.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AutoDeliveryContext _dbContext;
        public Repository(AutoDeliveryContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<TEntity> GetList()
        {
            var dbSet = _dbContext.Set<TEntity>();
            return dbSet.ToList();
        }

        public List<TEntity> GetList(Func<TEntity,bool> predicate)
        {
           var dbSet = _dbContext.Set<TEntity>();
            return dbSet.Where(predicate).ToList();
        }


        public async Task<List<TEntity>> GetListAsync(PageWithSortDto pageWithSortDto)
        {
            int skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;
           var dbSet = _dbContext.Set<TEntity>();
            if (pageWithSortDto.OrderType == OrderType.Asc)
            {
                return await dbSet.OrderBy("Id",false).Skip(skip).Take(pageWithSortDto.PageSize).ToListAsync();
            }
            else
            {
                return await dbSet.OrderBy("Id",true).Skip(skip).Take(pageWithSortDto.PageSize).ToListAsync();
            }
        }

        // 获取queryable
        public IQueryable<TEntity> GetQueryable()
        {
            var dbSet = _dbContext.Set<TEntity>();
            return dbSet;
        }

        public async Task<List<TEntity>> GetListAsync()
        {
            return await GetListAsync(new PageWithSortDto());
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity,bool>> predicate)
        {
            var dbSet = _dbContext.Set<TEntity>();
            return await dbSet.Where(predicate).ToListAsync();
        }


        public async Task<List<TEntity>>  GetListAsync(Expression<Func<TEntity,bool>> predicate,string sort,int pageIndex,int pageSize)
        {
            int skip = (pageIndex - 1) * pageSize;
           var dbSet = _dbContext.Set<TEntity>();
            return await dbSet.Where(predicate).OrderBy(m => sort).Skip(skip).Take(pageSize).ToListAsync();
        }


        public TEntity Get(Func<TEntity,bool> predicate)
        {
            var dbSet = _dbContext.Set<TEntity>();
            return dbSet.FirstOrDefault(predicate);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity,bool>> predicate)
        {
            var dbSet = _dbContext.Set<TEntity>();
            return await dbSet.FirstOrDefaultAsync(predicate);
        }



        // Insert
        public TEntity Insert(TEntity entity)
        {
            var dbSet = _dbContext.Set<TEntity>();
            var res = dbSet.Add(entity).Entity;
            _dbContext.SaveChangesAsync();
            return res;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var dbSet = _dbContext.Set<TEntity>();
            var res = (await dbSet.AddAsync(entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return res;
        }


        // Delete
        public TEntity Delete(TEntity entity)
        {
            var dbSet = _dbContext.Set<TEntity>();
            var res = dbSet.Remove(entity).Entity;
            _dbContext.SaveChanges();
            return res;
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            var dbSet = _dbContext.Set<TEntity>();
            var res = dbSet.Remove(entity).Entity;
            await _dbContext.SaveChangesAsync();
            return res;
        }


        // Update
         public TEntity Update(TEntity entity)
        {
            // _context.Entry<TEntity>(entity).Property("Id").IsModified = false;
            var dbSet = _dbContext.Set<TEntity>();
            var res = dbSet.Update(entity).Entity;
            _dbContext.SaveChanges();
            return res;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            // _context.Entry<TEntity>(entity).Property("Id").IsModified = false;
            var dbSet = _dbContext.Set<TEntity>();
            var res = dbSet.Update(entity).Entity;
            await _dbContext.SaveChangesAsync();
            return res;
        }

    }
}