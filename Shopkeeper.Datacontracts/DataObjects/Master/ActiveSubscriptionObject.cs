
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class ActiveSubscriptionObject
    {
        public long ActiveSubscriptionId { get; set; }
        public long StoreIdId { get; set; }
        public long SubscriptionPackageId { get; set; }
        public int SubscriptionStatus { get; set; }
        public System.DateTime DateSubscribed { get; set; }
        public System.DateTime LastUpdated { get; set; }

        public virtual StoreObject StoreObject { get; set; }
        public virtual SubscriptionPackageObject SubscriptionPackage { get; set; }
    }
}
