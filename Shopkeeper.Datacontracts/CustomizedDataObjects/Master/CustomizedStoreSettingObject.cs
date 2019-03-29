
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class StoreSettingObject
    {
        public string DBScriptPath { get; set; }

        public string SqlConnectionString { get; set; }
        public string EntityConnectionString { get; set; }
        public string DomainExtension { get; set; }
        public string StoreName { get; set; }
        public int TotalOutlets { get; set; }
        public string StoreAlias { get; set; }
        public string CompanyName { get; set; }
        public string CustomerEmail { get; set; }
        public int SubscriptionStatus { get; set; }
        public string BillingCycleCode { get; set; }
        public string StoreLogoPath { get; set; }
        public string DefaultCurrency { get; set; }
        public string DefaultCurrencySymbol { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public string SecreteKey { get; set; }
        public string Slogan { get; set; }
        public string Url { get; set; }
        public string StoreEmail { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public bool Ltpd { get; set; }
        public string StoreAddress { get; set; }
        public string StoreLogo { get; set; }
        public string LoggedOnUser { get; set; }
        public long TicketTimeOut { get; set; }
        public string StreetNo { get; set; }
        public string OriginalLogoPath { get; set; }
    }

}
