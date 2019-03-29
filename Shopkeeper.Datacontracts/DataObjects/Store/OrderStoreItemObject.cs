
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public  class OrderStoreItemObject
    {
        public long OrderProductId { get; set; }
        public long OrderId { get; set; }
        public long ProductStockId { get; set; }
        public double UnitPrice { get; set; }
        public int QuantityOrdered { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string Discount { get; set; }
    
        public virtual OrderObject OrderObject { get; set; }
        public virtual StoreItemStockObject ProductStockObject { get; set; }
    }
}
