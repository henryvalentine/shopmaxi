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
    
    public partial class Invoice
    {
        public long InvoiceId { get; set; }
        public string ReferenceCode { get; set; }
        public long PurchaseOrderId { get; set; }
        public int StatusCode { get; set; }
        public System.DateTime DueDate { get; set; }
        public System.DateTime DateSent { get; set; }
        public string Attachment { get; set; }
    
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
