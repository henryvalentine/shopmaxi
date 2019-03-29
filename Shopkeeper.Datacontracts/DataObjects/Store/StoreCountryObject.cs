
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreCountryObject
    {
        public long StoreCountryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StoreStateObject> StoreStatesObjects { get; set; }
        public virtual ICollection<StoreCurrencyObject> StoreCurrencyObjects { get; set; }
    }
}

