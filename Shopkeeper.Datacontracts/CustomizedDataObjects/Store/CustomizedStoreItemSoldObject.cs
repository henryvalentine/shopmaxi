
using System;
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemSoldObject
    {
        public string ItemSoldName { get; set; }
        public string UoMCode { get; set; }
        public string StoreItemName { get; set; }
        public string DateSoldStr { get; set; }
        public double TotalPrice { get; set; }
        public long PurchaseOrderItemId { get; set; }
        public long PurchaseOrderId { get; set; }
        public string AmountSoldStr { get; set; }
        public string AmountBoughtStr { get; set; }
        public double AmountBought { get; set; }
        public string QuantityLeftStr { get; set; }
        public string RateStr { get; set; }
        public string QuantitySoldStr { get; set; }
        public string QuantityBoughtStr { get; set; }
        public double QuantityBought { get; set; } 
        public double UnitPrice { get; set; }
        public double QuantityLeft { get; set; }
        public double? QuantityAlreadySold { get; set; }
        public string Sku { get; set; }
        public string Employee { get; set; }
        public string ImagePath { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public string StockValueStr { get; set; }
        public string DateBoughtStr { get; set; }
        public DateTime DateBought { get; set; }
        public DateTime DateDelivered { get; set; }
        public double QuantityOrdered { get; set; }
        public double QuantityInStock { get; set; }

        public string Reason { get; set; }
        public double? CostPrice { get; set; }
        public string BrandName { get; set; }
        public double AmountPaid { get; set; }
        public double NetAmount { get; set; }
        public double CostOfSale { get; set; }
        public double Profit { get; set; }
        public string CostOfSaleStr { get; set; }
        public double StockValue { get; set; }
        public string CategoryName { get; set; }
        public int IssueTypeId { get; set; }
        public int? StoreItemVariationValueId { get; set; }
        public double ReOrderQuantity { get; set; }
        public double ReturnedQuantity { get; set; }
        public long StoreItemCategoryId { get; set; }
        public double ReOrderLevel { get; set; }
        public string ReOrderQuantityStr { get; set; }
        public string ReOrderLevelStr { get; set; }
        public virtual List<ReturnedProductObject> ReturnedProductObjects { get; set; }
        public virtual List<ItemPriceObject> ItemPriceObjects { get; set; }
    }

} 

 