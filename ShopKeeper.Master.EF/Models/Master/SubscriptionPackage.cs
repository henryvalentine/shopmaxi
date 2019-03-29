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
    
    public partial class SubscriptionPackage
    {
        public SubscriptionPackage()
        {
            this.ActiveSubscriptions = new HashSet<ActiveSubscription>();
            this.PackagePricings = new HashSet<PackagePricing>();
            this.Stores = new HashSet<Store>();
            this.StoreSubscriptionHistories = new HashSet<StoreSubscriptionHistory>();
        }
    
        public long SubscriptionPackageId { get; set; }
        public string PackageTitle { get; set; }
        public long FileStorageSpace { get; set; }
        public int NumberOfStoreProducts { get; set; }
        public int NumberOfOutlets { get; set; }
        public int Registers { get; set; }
        public int NumberOfUsers { get; set; }
        public bool UseReportBuilder { get; set; }
        public bool GenerateReports { get; set; }
        public int MaximumTransactions { get; set; }
        public string Note { get; set; }
        public bool LiveChat { get; set; }
        public bool EmailSupport { get; set; }
        public bool TelephoneSupport { get; set; }
        public bool DedicatedAccountManager { get; set; }
        public long MaximumCustomer { get; set; }
        public double TransactionFee { get; set; }
    
        public virtual ICollection<ActiveSubscription> ActiveSubscriptions { get; set; }
        public virtual ICollection<PackagePricing> PackagePricings { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<StoreSubscriptionHistory> StoreSubscriptionHistories { get; set; }
    }
}
