using System;
using ShopkeeperStore.EF.Models.Store;

namespace ShopkeeperStore.EF.Extensions
{
    interface IStoreEntityExtension : IDisposable
    {
        ShopKeeperStoreEntities ShopkeeperStoreContext { get; }

    }
}
