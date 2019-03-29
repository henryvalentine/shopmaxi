
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StockItemVariationObject
    {
        public long Id { get; set; }
        public long StoreItemStockId { get; set; }
        public int VariationId { get; set; }
        public int VariationValueId { get; set; }
        public string SampleImagePath { get; set; }

        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
        public virtual StoreItemVariationObject StoreItemVariationObject { get; set; }
        public virtual StoreItemVariationValueObject StoreItemVariationValueObject { get; set; }
    }
}
