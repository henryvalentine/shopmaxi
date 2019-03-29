
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoreItemStockObject
    {
        public long StoreItemStockId { get; set; }
        public Nullable<int> StoreOutletId { get; set; }
        public string SKU { get; set; }
        public long StoreItemId { get; set; }
        public double ReorderLevel { get; set; }
        public double ReorderQuantity { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public string ShelfLocation { get; set; }
        public long StoreCurrencyId { get; set; }
        public Nullable<int> StoreItemVariationId { get; set; }
        public Nullable<int> StoreItemVariationValueId { get; set; }
        public double QuantityInStock { get; set; }
        public double TotalQuantityAlreadySold { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<double> CostPrice { get; set; }
        public bool PublishOnline { get; set; }
        public bool Discontinued { get; set; }

        public virtual ICollection<DailyInventoryObject> DailyInventoryObjects { get; set; }
        public virtual ICollection<EstimateItemObject> EstimateItemObjects { get; set; }
        public virtual ICollection<ItemPriceObject> ItemPriceObjects { get; set; }
        public virtual ICollection<ItemReviewObject> ItemReviewObjects { get; set; }
        public virtual ICollection<PurchaseOrderItemObject> PurchaseOrderItemObjects { get; set; }
        public virtual ICollection<ReturnedProductObject> ReturnedProductObjects { get; set; }
        public virtual ICollection<StockUploadObject> StockUploadObjects { get; set; }
        public virtual StoreCurrencyObject StoreCurrencyObject { get; set; }
        public virtual StoreItemObject StoreItemObject { get; set; }
        public virtual ICollection<StoreItemIssueObject> StoreItemIssueObjects { get; set; }
        public virtual ICollection<StoreItemSoldObject> StoreItemSoldObjects { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual StoreItemVariationObject StoreItemVariationObject { get; set; }
        public virtual StoreItemVariationValueObject StoreItemVariationValueObject { get; set; }

    }
}
