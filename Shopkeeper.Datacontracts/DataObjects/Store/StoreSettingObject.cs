
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class StoreSettingObject
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string StoreAlias { get; set; }
        public string CompanyName { get; set; }
        public string StoreEmail { get; set; }
        public string StoreLogoPath { get; set; }
        public long? DefaultCurrencyId { get; set; }
        public double VAT { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DbSyncTimeShedule { get; set; }
        public bool DeductStockAtSalesPoint { get; set; }
        public virtual StoreCurrencyObject StoreCurrencyObject { get; set; }
    }
    

}



