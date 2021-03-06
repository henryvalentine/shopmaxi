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
    
    public partial class InvoiceRequest
    {
        public InvoiceRequest()
        {
            this.InvoiceRequestItems = new HashSet<InvoiceRequestItem>();
        }
    
        public long Id { get; set; }
        public long SaleId { get; set; }
        public long RequestedById { get; set; }
        public System.DateTime DateRequested { get; set; }
    
        public virtual Sale Sale { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<InvoiceRequestItem> InvoiceRequestItems { get; set; }
    }
}
