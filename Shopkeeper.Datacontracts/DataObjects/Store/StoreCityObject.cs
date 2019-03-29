
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreCityObject
    {
        public long StoreCityId { get; set; }
        public string Name { get; set; }
        public long StoreStateId { get; set; }

        public virtual ICollection<StoreAddressObject> StoreAddressObjects { get; set; }
        public virtual StoreStateObject StoreState { get; set; }
        public virtual ICollection<DeliveryAddressObject> DeliveryAddressObjects { get; set; }
    }
}

