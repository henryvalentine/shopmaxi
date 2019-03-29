

using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreCurrencyObject
    {
        public long StoreCurrencyId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public long StoreCountryId { get; set; }
        public bool IsDefaultCurrency { get; set; }

        public virtual StoreCountryObject StoreCountryObject { get; set; }
        public virtual ICollection<StoreItemStockObject> StoreItemStockObjects { get; set; }
    }
}
