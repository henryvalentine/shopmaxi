using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
        public enum SortOrder { Ascending, Descending }

        public interface IShopkeeperRepository<T> where T : class
        {
            DbContext RepositoryContext();
            T Add(T entity);
            T Remove(T entity);
            T Remove(object key);
            List<T> RemoveWithResultList(Expression<Func<T, bool>> predicate);
            T Update(T entity);
            Int32 Count();
            Int32 Count(Expression<Func<T, bool>> predicate);
            IQueryable<T> GetAll();
            IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
            IQueryable<T> GetAll(string includeProperties);
            IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, string includeProperties);
            T GetById(object key);
            //T Get(string includeProperties);
            T Get(Expression<Func<T, bool>> predicate, string includeProperties);
            
            IEnumerable<T> GetWithPaging<TOrderBy>(Expression<Func<T, bool>> criteria, Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, string includeProperties = "");
            IEnumerable<T> GetWithPaging<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, string includeProperties);
            
            IEnumerable<T> GetWithPaging<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize);
            IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
            IEnumerable<T> Get<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder = SortOrder.Ascending);
            IEnumerable<T> Get<TOrderBy>(Expression<Func<T, bool>> criteria, Expression<Func<T, TOrderBy>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder = SortOrder.Ascending, string includeProperties = "");
        }
    
}
