using System;
using ShopkeeperStore.EF.Extensions;

namespace ShopkeeperStore.EF.Models.Store
{
    public partial class ShopKeeperStoreEntities : IStoreEntityExtension
    {
        public ShopKeeperStoreEntities(string conn) : base(conn)
        {
            ShopkeeperStoreContext = this;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        void IDisposable.Dispose()
        {
            ShopkeeperStoreContext.Dispose();
        }

        public ShopKeeperStoreEntities ShopkeeperStoreContext { get; private set; }
    }
}
