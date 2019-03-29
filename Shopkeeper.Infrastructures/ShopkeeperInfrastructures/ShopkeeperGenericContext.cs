using System;
using System.Data.Entity;
using ShopkeeperStore.EF.Models;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
    public class ShopkeeperGenericContext : DbContext, IShopkeeperContext
    {
        public ShopkeeperGenericContext(string contextConnector) : base(contextConnector)
        {
            ShopkeeperContext = new DbContext(contextConnector);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
         
        public ShopkeeperGenericContext(DbContext context) : base(context.Database.Connection.ConnectionString)
        {
            ShopkeeperContext = context;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        void IDisposable.Dispose()
        {
            ShopkeeperContext.Dispose();
        }

       public DbContext ShopkeeperContext { get; private set; }
    }
}
