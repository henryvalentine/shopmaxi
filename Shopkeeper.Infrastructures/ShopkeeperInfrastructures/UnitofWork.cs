using System;
using System.Data.Entity;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
   public class UnitOfWork : IUnitofWork, IDisposable
    {
        private readonly DbContext _context;
        public UnitOfWork(string contextConnectror)
        {
            _context = new DbContext(contextConnectror);
        }

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }
       
       public void SaveChanges()
       {
           _context.SaveChanges();
       }

       public DbContext Context { get { return _context; } }
       
       #region Implementation of IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing) return;
			if (_disposed) return;
			 _context.Dispose();
			 _disposed = true;
		}

		private bool _disposed;

		~UnitOfWork()
		{
			 Dispose(false);
		}

		#endregion
    }
}

