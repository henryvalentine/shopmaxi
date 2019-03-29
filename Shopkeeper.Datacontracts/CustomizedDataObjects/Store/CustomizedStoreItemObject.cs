
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemObject
    {
        public string StoreItemBrandName { get; set; }
        public string StoreItemTypeName { get; set; }
        public string StoreItemCategoryName { get; set; }
        public string ChartOfAccountTypeName { get; set; }
        public string ParentItemName { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public string CurrencySymbol { get; set; }
        public long StoreItemStockId { get; set; }
    }
}
