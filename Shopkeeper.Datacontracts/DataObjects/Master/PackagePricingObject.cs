
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class PackagePricingObject
    {
        public long PackagePricingId { get; set; }
        public long SubscriptionPackageId { get; set; }
        public long BillingCycleId { get; set; }
        public double Price { get; set; }
    
        public virtual BillingCycleObject BillingCycleObject { get; set; }
        public virtual SubscriptionPackageObject SubscriptionPackageObject { get; set; }

    }
}
