
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;

    public class EmployeeAssigmentObject
    {
        public long EmployeeAssignmentId { get; set; }
        public long EmployeeId { get; set; }
        public long JobRoleId { get; set; }
        public long StoreOutletId { get; set; }
        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public byte[] LastUpdate { get; set; }

        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual JobRoleObject JobRoleObject { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
    }
}
