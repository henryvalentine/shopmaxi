
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class PurchaseOrderItemDeliveryObject
    {
        public string DateDeliveredStr { get; set; }
        public long PurchaseOrderId { get; set; }
        public long StoreItemId { get; set; }
        public int StoreOutletId { get; set; }
        public long StoreItemStockId { get; set; }
        public string StoreItemName { get; set; }

        public double Price { get; set; }
        public double MinimumQuantity { get; set; }
        public double TotalCost { get; set; }
        public long ItemPriceId { get; set; }

        public string SerialNumber { get; set; }
        public string DeliveryStatus { get; set; }
        public double QuantityOrdered { get; set; }
        public int PurchaseOrderItemStatusCode { get; set; }
        public double CostPrice { get; set; }
        public string ExpiryDateStr { get; set; }
        public string EmployeeName { get; set; }

         
    }
}

