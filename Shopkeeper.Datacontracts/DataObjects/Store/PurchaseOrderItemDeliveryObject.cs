
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class PurchaseOrderItemDeliveryObject
    {
        public long PurchaseOrderItemDeliveryId { get; set; }
        public long PurchaseOrderItemId { get; set; }
        public double QuantityDelivered { get; set; }
        public System.DateTime DateDelivered { get; set; }
        public long ReceivedById { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual PurchaseOrderItemObject PurchaseOrderItemObject { get; set; }
    }
}

