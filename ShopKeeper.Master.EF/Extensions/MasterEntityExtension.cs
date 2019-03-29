using System;
using ShopKeeper.Master.EF.Extensions;
namespace ShopKeeper.Master.EF.Models.Master
{
    public partial class ShopKeeperMasterEntities : IShopKeeperMasterEntities
    {
        public ShopKeeperMasterEntities(string conn) : base(conn)
        {
            ShopkeeperMasterContext = this;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        void IDisposable.Dispose()
        {
           ShopkeeperMasterContext.Dispose();
        }
        public ShopKeeperMasterEntities ShopkeeperMasterContext { get; private set; }
    }
}
