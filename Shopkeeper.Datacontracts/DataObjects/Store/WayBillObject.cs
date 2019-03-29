namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public  class WayBillObject
    {
        public long WayBillId { get; set; }
        public long AuthorisedById { get; set; }
        public long SourceId { get; set; }
        public long DeliveryId { get; set; }
        public int TotalItems { get; set; }
        public System.DateTime DatePacked { get; set; }
        public long PackedById { get; set; }
        public long CheckedById { get; set; }
    
        public virtual DeliveryObject DeliveryObject { get; set; }
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual EmployeeObject EmployeeObject1 { get; set; }
        public virtual ICollection<WayBillItemObject> WayBillItemObjects { get; set; }
    }
}
