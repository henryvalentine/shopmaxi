
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ItemReviewObject
    {
        public long ItemReviewId { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewComment { get; set; }
        public int Rating { get; set; }
        public long StoreItemStockId { get; set; }
    
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
    }
}
