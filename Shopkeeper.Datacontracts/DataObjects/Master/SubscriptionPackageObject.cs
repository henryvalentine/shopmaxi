
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;

    public partial class SubscriptionPackageObject
    {
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

        public virtual ICollection<ActiveSubscriptionObject> ActiveSubscriptions { get; set; }
        public virtual ICollection<PackagePricingObject> PackagePricings { get; set; }
        public virtual ICollection<StoreSubscriptionHistoryObject> StoreSubscriptionHistories { get; set; }

    }
}
