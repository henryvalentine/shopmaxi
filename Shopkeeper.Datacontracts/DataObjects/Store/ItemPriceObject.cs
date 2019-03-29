
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ItemPriceObject
    {
        public long ItemPriceId { get; set; }
        public long StoreItemStockId { get; set; }
        public double Price { get; set; }
        public double MinimumQuantity { get; set; }
        public string Remark { get; set; }
        public long UoMId { get; set; }

        public virtual StoreItemStockObject StoreItemStock { get; set; }
        public virtual UnitsOfMeasurementObject UnitsOfMeasurement { get; set; }
    }
}

