
using System;
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class PurchaseOrderItemObject
    {
        public long PurchaseOrderItemId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long StoreItemStockId { get; set; }
        public string SerialNumber { get; set; }
        public double QuantityOrdered { get; set; }
        public double QuantityDelivered { get; set; }
        public int StatusCode { get; set; }
        public double CostPrice { get; set; }
        public double QuantityInStock { get; set; }
        

        public virtual PurchaseOrderObject PurchaseOrderObject { get; set; }
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
        public virtual ICollection<PurchaseOrderItemDeliveryObject> PurchaseOrderItemDeliveryObjects { get; set; }

    }

}


