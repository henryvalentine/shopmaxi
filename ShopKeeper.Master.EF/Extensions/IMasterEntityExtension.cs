using ShopKeeper.Master.EF.Models.Master;
using System;

namespace ShopKeeper.Master.EF.Extensions
{
    interface IShopKeeperMasterEntities : IDisposable
    {
        ShopKeeperMasterEntities ShopkeeperMasterContext { get; }
        
    }
}
