
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;

    public partial class StoreObject
    {
        public long StoreId { get; set; }
        public string StoreName { get; set; }
        public int TotalOutlets { get; set; }
        public string StoreAlias { get; set; }
        public string CompanyName { get; set; }
        public string CustomerEmail { get; set; }
        public int SubscriptionStatus { get; set; }
        public string BillingCycleCode { get; set; }
        public string StoreLogoPath { get; set; }
        public string DefaultCurrency { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public string SecreteKey { get; set; }
        public long SubscriptionPackageId { get; set; }

        public virtual ICollection<ActiveSubscriptionObject> ActiveSubscriptionObjects { get; set; }
        public virtual SubscriptionPackageObject SubscriptionPackageObject { get; set; }
        public virtual ICollection<StoreBankAccountObject> StoreBankAccountObjects { get; set; }
        public virtual ICollection<StorePaymentGatewayObject> StorePaymentGatewayObjects { get; set; }
        public virtual StoreSettingObject StoreSettingObject { get; set; }
        public virtual ICollection<StoreSubscriptionHistoryObject> StoreSubscriptionHistoryObjects { get; set; }
    }
}
