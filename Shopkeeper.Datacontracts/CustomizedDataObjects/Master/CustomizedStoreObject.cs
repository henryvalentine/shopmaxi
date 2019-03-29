
using System;
using Shopkeeper.DataObjects.DataObjects.Store;

namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class StoreObject
    {
        public string CurrencyName { get; set; }
        public int PaymentMethodId { get; set; }
        public double Amount { get; set; }
        public long StorageSize { get; set; }
        public int Duration { get; set; }
        public bool IsTrial { get; set; }
        public long BillingCycleId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentOption { get; set; }
        public string PackageName { get; set; }
        public bool IsBankOption { get; set; }
    }
}
