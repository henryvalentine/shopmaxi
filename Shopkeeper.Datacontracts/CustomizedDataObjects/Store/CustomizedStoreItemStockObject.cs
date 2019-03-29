
using System;
using System.Collections.Generic;
using System.Web;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemStockObject
    {
        public string VariationProperty { get; set; }
        public string VariationValue { get; set; }
        public string StoreItemName { get; set; } 
        public string StoreOutletName { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public string FeedbackMessage { get; set; }
        public string ExpiryDate { get; set; }
        public string QuantityInStockStr { get; set; }
        public string QuantitySoldStr { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public int MinimumQuantity { get; set; }
        public long UnitOfMeasurementId { get; set; }
        public string UoMCode { get; set; }
        public string BrandName { get; set; }
        public string TypeName { get; set; }
        public string CategoryName { get; set; } 
        public long ItemPriceId { get; set; }
        public long StoreItemBrandId { get; set; }
        public long PurchaseOrderItemId { get; set; }
        
        public long StoreItemTypeId { get; set; }
        public long StoreItemCategoryId { get; set; }
        public string Description { get; set; }
        public Nullable<long> ParentItemId { get; set; }
        public string TechSpechs { get; set; }

        public List<StoreItemObject> SimilarItems { get; set; }
        public List<ItemVariationObject> ItemVariationObjects { get; set; }
        public List<VariationObject> VariationObjects { get; set; }
        public HttpPostedFileBase VariantImage { get; set; }

        public List<SessionObj> SessionObjs { get; set; }

        public double StockValue { get; set; }
        public string StockValueStr { get; set; }
        public string CostPriceStr { get; set; }
        public string TotalQuantityAlreadySoldStr { get; set; }
        
        public string ReOrderQuantityStr { get; set; }
        public string ReOrderLevelStr { get; set; }
        
    }

    public class ItemVariationObject
    {
        public string StoreItemVariationName { get; set; }
        public string StoreItemVariationValueName { get; set; }
        public int StoreItemVariationId { get; set; }
        public int StoreItemVariationValueId { get; set; }

    }
    
    public class VariationObject
    {
        public string StoreItemVariation { get; set; }
        public int StoreItemVariationId { get; set; }
        public string ModelValue { get; set; }
        public List<StoreItemVariationValueObject> StoreItemVariationValueObjects { get; set; }
    }

    public class CartItemObject
    {
        public long ShoppinCartItemId { get; set; }
        public double QuantityOrdered { get; set; }
    }

    public class DailySaleReport
    {
        public double TotalDailySales { get; set; }
        public double TotalDailyRefunds { get; set; }
        public int DailySalesCount { get; set; }
        public int TotalItemsToReorder { get; set; }
        public int TotalDueInvoices { get; set; }
        public List<StoreItemSoldObject> HighSalingStockList { get; set; }
        public List<CustomerInvoiceObject> CustomerPerformances { get; set; }
        public List<SaleComparism> SaleComparism { get; set; }
    }

    public class SaleComparism
    {
        public double SaleMagnitude { get; set; }
        public string DateStr { get; set; }
    }
}





