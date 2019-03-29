using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
    interface IShopkeeperContext : IDisposable
    {
        DbContext ShopkeeperContext { get; }
        
    }
}
