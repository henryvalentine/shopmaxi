using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shopkeeper.Datacontracts.DataObjects.Store
{
    public class SubscriptionPackageObject
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

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string CustomerEmail { get; set; }
        public long BillingCycleId { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public int Duration { get; set; }
        public List<PackagePricingObject> PackagePricings { get; set; } 
    }

    public class PackagePricingObject
    {
        public long PackagePricingId { get; set; }
        public long SubscriptionPackageId { get; set; }
        public long BillingCycleId { get; set; }
        public double Price { get; set; }
        public string SubscriptionPackageTitle { get; set; }
        public string BillingCycleName { get; set; }
        public string BillingCycleCode { get; set; }
        public int Duration { get; set; }

    }
}
