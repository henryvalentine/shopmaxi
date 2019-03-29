
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial  class DeliveryObject
    {
        public long DeliveryId { get; set; }
        public string TrackingNumber { get; set; }
        public long PurchaseOrderId { get; set; }
        public System.DateTime ExpectedDeliveryDate { get; set; }
        public Nullable<System.DateTime> AcutalDeliveryDate { get; set; }
        public long ReceivedById { get; set; }

        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual PurchaseOrderObject PurchaseOrderObject { get; set; }
        public virtual ICollection<WayBillObject> WayBillsObject { get; set; }
    }
}

