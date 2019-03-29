
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public  class OrderObject
    {
        public long OrderId { get; set; }
        public long StoreOutletId { get; set; }
        public long EmployeeObjectId { get; set; }
        public long CustomerId { get; set; }
        public double OrderValue { get; set; }
        public int Status { get; set; }
        public System.DateTime OrderDate { get; set; }
        public byte[] LastUpdated { get; set; }
    
        public virtual CustomerObject CustomerObject { get; set; }
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual ICollection<OrderPaymentObject> OrderPaymentObjects { get; set; }
        public virtual ICollection<OrderStoreItemObject> OrderProductObjects { get; set; }
    }
}
