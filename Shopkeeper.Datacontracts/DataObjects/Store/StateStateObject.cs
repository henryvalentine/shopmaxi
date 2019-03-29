
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class StoreStateObject
    {
        public long StoreStateId { get; set; }
        public string Name { get; set; }
        public long StoreCountryId { get; set; }

        public virtual ICollection<StoreCityObject> StoreCityObjects { get; set; }
        public virtual StoreCountryObject StoreCountryObject { get; set; }
    }
}


