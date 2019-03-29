
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class StoreSubscriptionHistoryObject
    {
        public long StoreSubscriptionHistoryId { get; set; }
        public long StoreId { get; set; }
        public long SubscriptionPackageId { get; set; }
        public long PaymentId { get; set; }
        public System.DateTime DateSubscribed { get; set; }
        public System.DateTime SubscriptionExpiryDate { get; set; }
        public int Duration { get; set; }
        public string Note { get; set; }
    
        public virtual StoreObject StoreObject { get; set; }
        public virtual SubscriptionPackageObject SubscriptionPackageObject { get; set; }
    }
}
