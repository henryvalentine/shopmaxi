
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public class DailyInventoryObject
    {
        public long DailyInventoryId { get; set; }
        public long ProductStockId { get; set; }
        public int StockLevel { get; set; }
        public System.DateTime DateTaken { get; set; }
        public byte[] DateCreated { get; set; }

        public virtual StoreItemStockObject Object { get; set; }
    }
}
