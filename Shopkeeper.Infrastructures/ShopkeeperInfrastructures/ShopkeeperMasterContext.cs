using System;
using System.Data.Entity;
using ShopkeeperStore.EF.Models.Master;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
    public class ShopkeeperMasterContext : ShopKeeperMasterEntities, IShopkeeperContext
    {
        public ShopkeeperMasterContext(string contextConnector)
        {
            ShopkeeperContext = this;
            Database.Connection.ConnectionString = contextConnector;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public ShopkeeperMasterContext(DbContext context)
        {
            ShopkeeperContext = context;
            Database.Connection.ConnectionString = context.Database.Connection.ConnectionString;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public ShopkeeperMasterContext()
        {
            ShopkeeperContext = this;
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
