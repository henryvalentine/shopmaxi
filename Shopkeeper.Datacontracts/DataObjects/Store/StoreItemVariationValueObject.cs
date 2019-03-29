

using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemVariationValueObject
    {
        public int StoreItemVariationValueId { get; set; }
        public string Value { get; set; }

        public virtual ICollection<StoreItemStockObject> StoreItemStockObjects { get; set; }
    }
}
