//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopkeeperStore.EF.Models.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoreItemStock
    {
        public StoreItemStock()
        {
            this.DailyInventories = new HashSet<DailyInventory>();
            this.EstimateItems = new HashSet<EstimateItem>();
            this.ItemPrices = new HashSet<ItemPrice>();
            this.ItemReviews = new HashSet<ItemReview>();
            this.PurchaseOrderItems = new HashSet<PurchaseOrderItem>();
            this.ReturnedProducts = new HashSet<ReturnedProduct>();
            this.ShopingCartItems = new HashSet<ShopingCartItem>();
            this.StockUploads = new HashSet<StockUpload>();
            this.StoreItemIssues = new HashSet<StoreItemIssue>();
            this.StoreItemSolds = new HashSet<StoreItemSold>();
            this.TransferNoteItems = new HashSet<TransferNoteItem>();
        }
    
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
    
        public virtual ICollection<DailyInventory> DailyInventories { get; set; }
        public virtual ICollection<EstimateItem> EstimateItems { get; set; }
        public virtual ICollection<ItemPrice> ItemPrices { get; set; }
        public virtual ICollection<ItemReview> ItemReviews { get; set; }
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public virtual ICollection<ReturnedProduct> ReturnedProducts { get; set; }
        public virtual ICollection<ShopingCartItem> ShopingCartItems { get; set; }
        public virtual ICollection<StockUpload> StockUploads { get; set; }
        public virtual StoreCurrency StoreCurrency { get; set; }
        public virtual StoreItem StoreItem { get; set; }
        public virtual ICollection<StoreItemIssue> StoreItemIssues { get; set; }
        public virtual ICollection<StoreItemSold> StoreItemSolds { get; set; }
        public virtual StoreOutlet StoreOutlet { get; set; }
        public virtual StoreItemVariation StoreItemVariation { get; set; }
        public virtual StoreItemVariationValue StoreItemVariationValue { get; set; }
        public virtual ICollection<TransferNoteItem> TransferNoteItems { get; set; }
    }
}
