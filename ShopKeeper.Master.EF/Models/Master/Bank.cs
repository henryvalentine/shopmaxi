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
    
    public partial class Bank
    {
        public Bank()
        {
            this.StoreBankAccounts = new HashSet<StoreBankAccount>();
        }
    
        public long BankId { get; set; }
        public string SortCode { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string LogoPath { get; set; }
        public string LastUpdated { get; set; }
    
        public virtual ICollection<StoreBankAccount> StoreBankAccounts { get; set; }
    }
}