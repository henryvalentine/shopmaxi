using System;
using ShopkeeperStore.EF.Models.Master;

namespace ShopkeeperStore.EF.Extension
{
    interface IShopKeeperMasterEntities : IDisposable
    {
        ShopKeeperMasterEntities ShopkeeperMasterContext { get; }
        
    }
}
