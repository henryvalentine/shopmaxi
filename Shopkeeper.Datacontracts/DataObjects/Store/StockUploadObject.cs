
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StockUploadObject
    {
        public long StockUploadId { get; set; }
        public long StoreItemStockId { get; set; }
        public string ImagePath { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public int ImageViewId { get; set; }

        public virtual ImageViewObject ImageView { get; set; }
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
    }
}

