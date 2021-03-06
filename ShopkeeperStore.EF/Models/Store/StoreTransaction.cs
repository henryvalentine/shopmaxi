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
    
    public partial class StoreTransaction
    {
        public StoreTransaction()
        {
            this.PurchaseOrderPayments = new HashSet<PurchaseOrderPayment>();
            this.RefundNotes = new HashSet<RefundNote>();
            this.SaleTransactions = new HashSet<SaleTransaction>();
        }
    
        public long StoreTransactionId { get; set; }
        public int StoreTransactionTypeId { get; set; }
        public int StorePaymentMethodId { get; set; }
        public long EffectedByEmployeeId { get; set; }
        public double TransactionAmount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string Remark { get; set; }
        public int StoreOutletId { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual ICollection<PurchaseOrderPayment> PurchaseOrderPayments { get; set; }
        public virtual ICollection<RefundNote> RefundNotes { get; set; }
        public virtual ICollection<SaleTransaction> SaleTransactions { get; set; }
        public virtual StoreOutlet StoreOutlet { get; set; }
        public virtual StorePaymentMethod StorePaymentMethod { get; set; }
        public virtual StoreTransactionType StoreTransactionType { get; set; }
    }
}
