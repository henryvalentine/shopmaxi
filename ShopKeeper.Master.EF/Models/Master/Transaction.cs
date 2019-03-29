//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopKeeper.Master.EF.Models.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class Transaction
    {
        public long TransactionId { get; set; }
        public int TransactionTypeId { get; set; }
        public int PaymentMethodId { get; set; }
        public double Amount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string Remark { get; set; }
    
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }
}
