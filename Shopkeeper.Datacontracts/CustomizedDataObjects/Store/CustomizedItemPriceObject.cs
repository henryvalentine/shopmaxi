
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ItemPriceObject
    {
        public string UoMCode { get; set; }
        public string StoreItemStockName { get; set; }
        public string CurrencySymbol { get; set; }
        public string Description { get; set; }
        public long StoreItemId { get; set; }
        public string PriceStr { get; set; }
        public string MinimumQuantityStr { get; set; }
        public string SKU { get; set; }
        public string ProductCode { get; set; }
        public long StoreItemCategoryId { get; set; }
        public double QuantityInStock { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }

}
