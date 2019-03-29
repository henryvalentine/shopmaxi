
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class EmployeeObject
    {
        public long EmployeeId { get; set; }
        public long UserId { get; set; }
        public string EmployeeNo { get; set; }
        public System.DateTime DateHired { get; set; }
        public Nullable<System.DateTime> DateLeft { get; set; }
        public int StoreOutletId { get; set; }
        public long StoreAddressId { get; set; }
        public long StoreDepartmentId { get; set; }
        public int Status { get; set; }

        public virtual UserProfileObject UserProfileObject { get; set; }
        public virtual StoreDepartmentObject StoreDepartmentObject { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual UserProfileObject UserProfileObject1 { get; set; }
        public virtual ICollection<EmployeeAssigmentObject> EmployeeAssigmentObjects { get; set; }
        public virtual ICollection<EmployeeDocumentObject> EmployeeDocumentObjects { get; set; }
        public virtual ICollection<EmployeeSalesLogObject> EmployeeSalesLogObjects { get; set; }
        public virtual ICollection<PurchaseOrderObject> PurchaseOrderObjects { get; set; }
        public virtual ICollection<PurchaseOrderItemDeliveryObject> PurchaseOrderItemDeliveryObjects { get; set; }
        public virtual ICollection<SaleObject> SaleObjects { get; set; }
        public virtual ICollection<StoreTransactionObject> StoreTransactionObjects { get; set; }
        public virtual ICollection<WayBillObject> WayBillObjects { get; set; }
        public virtual ICollection<WayBillObject> WayBillsObject1 { get; set; }
    }

}
