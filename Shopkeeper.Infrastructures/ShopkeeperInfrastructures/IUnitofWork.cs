using System.Data.Entity;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
    public interface IUnitofWork
    {
        void SaveChanges();
        DbContext Context { get; }
    }
}
