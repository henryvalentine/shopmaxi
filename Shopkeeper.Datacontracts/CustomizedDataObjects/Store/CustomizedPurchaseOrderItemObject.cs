
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class PurchaseOrderItemObject
    {
        public string OutletName { get; set; }
        public string StoreItemName { get; set; }
        public string DeliveryStatus { get; set; }
        public string SupplierName { get; set; }
        public string AccountGroupName { get; set; }
        public string UoMCode { get; set; }
        public string TrackingNumber { get; set; }
        public string ImagePath { get; set; }
        public string DateDeliveredStr { get; set; }
        public double TotalQuantityDelivered { get; set; }

        public double Price { get; set; }
        public double MinimumQuantity { get; set; }
        public long ItemPriceId { get; set; }
        public DateTime DateDelivered { get; set; }
    }
}

