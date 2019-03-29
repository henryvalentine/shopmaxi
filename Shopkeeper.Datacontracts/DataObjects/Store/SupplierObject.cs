
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class SupplierObject
    {
        public long SupplierId { get; set; }
        public DateTime DateJoined { get; set; }
        public Nullable<DateTime> LastSupplyDate { get; set; }
        public string Note { get; set; }
        public string CompanyName { get; set; }
        public string TIN { get; set; }

        public virtual ICollection<PurchaseOrderObject> PurchaseOrderObjects { get; set; }
        public virtual ICollection<SupplierAddressObject> SupplierAddressObjects { get; set; }
    }
}

