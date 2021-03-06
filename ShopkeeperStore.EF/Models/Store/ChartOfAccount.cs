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
    
    public partial class ChartOfAccount
    {
        public ChartOfAccount()
        {
            this.PurchaseOrders = new HashSet<PurchaseOrder>();
            this.StoreItems = new HashSet<StoreItem>();
        }
    
        public int ChartOfAccountId { get; set; }
        public int AccountGroupId { get; set; }
        public string AccountType { get; set; }
        public string AccountCode { get; set; }
    
        public virtual AccountGroup AccountGroup { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<StoreItem> StoreItems { get; set; }
    }
}
