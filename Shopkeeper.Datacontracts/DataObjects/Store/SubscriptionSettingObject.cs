
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class SubscriptionSettingObject
    {
        public long Id { get; set; }
        public string StoreName { get; set; }
        public string StoreLogoPath { get; set; }
        public string DefaultCurrencySymbol { get; set; }
        public string StoreAddress { get; set; }
        public string StoreEmail { get; set; }
        public string PhoneNumber { get; set; }
    }

    public partial class SubscriptionSettingObject
    {
        public long StoreId { get; set; }
        public string SecreteKey { get; set; }
        public double DatabaseSpace { get; set; }
        public double FileStorageSpace { get; set; }
        public string Url { get; set; }
        public System.DateTime DateSubscribed { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public int SubscriptionStatus { get; set; }
    }
}

