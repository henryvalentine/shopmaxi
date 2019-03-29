//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopkeeperStore.EF.Models.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class Supplier
    {
        public Supplier()
        {
            this.PurchaseOrders = new HashSet<PurchaseOrder>();
            this.SupplierAddresses = new HashSet<SupplierAddress>();
        }
    
        public long SupplierId { get; set; }
        public System.DateTime DateJoined { get; set; }
        public Nullable<System.DateTime> LastSupplyDate { get; set; }
        public string Note { get; set; }
        public string CompanyName { get; set; }
        public string TIN { get; set; }
    
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<SupplierAddress> SupplierAddresses { get; set; }
    }
}
