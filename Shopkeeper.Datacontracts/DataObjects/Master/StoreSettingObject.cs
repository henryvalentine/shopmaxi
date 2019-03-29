
using System;
using Shopkeeper.DataObjects.DataObjects.Store;

namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class StoreSettingObject
    {
        public long StoreId { get; set; }
        public string InitialCatalog { get; set; }
        public string StorePsswd { get; set; }
        public string DataSource { get; set; }
        public string UserName { get; set; }
        public virtual StoreObject StoreObject { get; set; }

        public int Id { get; set; }
        public long? DefaultCurrencyId { get; set; }
        public double VAT { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DbSyncTimeShedule { get; set; }
        public bool DeductStockAtSalesPoint { get; set; }
        public virtual StoreCurrencyObject StoreCurrencyObject { get; set; }
    }
}

