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
    
    public partial class WayBillItem
    {
        public long WayBillItemId { get; set; }
        public long WayBillId { get; set; }
        public long StoreItemId { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
    
        public virtual StoreItem StoreItem { get; set; }
        public virtual WayBill WayBill { get; set; }
    }
}
