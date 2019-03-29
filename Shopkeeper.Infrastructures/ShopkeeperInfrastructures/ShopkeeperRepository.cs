using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Shopkeeper.Infrastructures.Utilities;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
    public class ShopkeeperRepository<T> : IShopkeeperRepository<T> where T : class
    {
        private readonly IUnitofWork _context ;
        private readonly DbSet<T> _dbSet;
        private readonly DbContext _dbContext;

        public ShopkeeperRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public ShopkeeperRepository(IUnitofWork unitOfWorkWork)
		{
            try
            {
                if (unitOfWorkWork == null) throw new ArgumentNullException("unitOfWorkWork");
                _context = unitOfWorkWork;
                _dbSet = _context.Context.Set<T>();
                _dbContext = _context.Context;
            }
            catch(Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
            }
		}
        
        public ShopkeeperRepository()
        {
            
        }

        public IUnitofWork Context
        {
            get { return _context; }
        }

        public T Add(T entity)
        {
           return _dbSet.Add(entity);
        }

        public T Remove(T entity)
        {
            return _dbSet.Remove(entity);
        }

        public T Remove(object key)
        {
            var entity = _dbSet.Find(key);
            return _dbSet.Remove(entity);
        }

        public List<T> RemoveWithResultList(Expression<Func<T, bool>> predicate)
        {
            var entityList = _dbSet.Where(predicate).ToList();
            var result = new List<T>();
            if (!entityList.Any()) return new List<T>();
            entityList.ForEach(m =>
            {
                var removedItem = _dbSet.Remove(m);
                result.Add(removedItem);
            });

            return result;
        }

        public T Update(T entity)
        {
            var updated = _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return updated;
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Count(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, string includeProperties)
        {
            var query = _dbSet.Where(predicate);
            if (!query.Any()) { return null; }
            if (string.IsNullOrEmpty(includeProperties)) { return query; }
            return includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public IQueryable<T> GetAll(string includeProperties)
        {
            var query = _dbSet.AsQueryable();
            if (!query.Any()) { return null; }
            if (string.IsNullOrEmpty(includeProperties)) { return query; }
            return includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
        public T GetById(object key)
        {
            return _dbSet.Find(key);
        }

        public T Get(Expression<Func<T, bool>> predicate, string includeProperties)
        {
            var query = _dbSet.Where(predicate); 
            if (string.IsNullOrEmpty(includeProperties)) { return query.ToList()[0]; }
            return includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).ToList()[0];
        }
        
        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return orderBy != null ? orderBy(query).ToList() : query.ToList();
        }

        public IEnumerable<T> Get<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder = SortOrder.Ascending)
        {
            return sortOrder == SortOrder.Ascending ? GetAll().OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable() : GetAll().OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
        }

        public IEnumerable<T> Get<TOrderBy>(Expression<Func<T, bool>> criteria, Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder = SortOrder.Ascending, string includeProperties = "")
        {
            var filtValue = GetAll(criteria, includeProperties);
            if (filtValue == null) { return null; }
            return sortOrder == SortOrder.Ascending ? filtValue.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable() : filtValue.OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
        }

        public IEnumerable<T> GetWithPaging<TOrderBy>(Expression<Func<T, bool>> criteria, Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, string includeProperties = "")
        {
            var query = criteria != null ? _dbSet.Where(criteria).OrderBy(orderBy).Skip((pageIndex)*pageSize).Take(pageSize) : _dbSet.OrderBy(orderBy).Skip((pageIndex)*pageSize).Take(pageSize);

            if (!string.IsNullOrEmpty(includeProperties) && !string.IsNullOrWhiteSpace(includeProperties))
            {
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
            
            return query.ToList();
        }

        public IEnumerable<T> GetWithPaging<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, string includeProperties)
        {
            if (includeProperties == null) throw new ArgumentNullException("includeProperties");
            var query = _dbSet.OrderBy(orderBy).Skip((pageIndex) * pageSize).Take(pageSize);

            if (!string.IsNullOrEmpty(includeProperties) && !string.IsNullOrWhiteSpace(includeProperties))
            {
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            return query.ToList();
        }
        
        public IEnumerable<T> GetWithPaging<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize)
        {
            var query = _dbSet.OrderBy(orderBy).Skip((pageIndex) * pageSize).Take(pageSize);
            return query.ToList();
        }

        public DbContext RepositoryContext()
        {
            return _dbContext;
        }

    }
}
