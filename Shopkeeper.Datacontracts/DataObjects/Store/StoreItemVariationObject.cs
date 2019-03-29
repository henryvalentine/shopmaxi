
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemVariationObject
    {
        public int StoreItemVariationId { get; set; }
        public string VariationProperty { get; set; }

        public virtual List<StoreItemStockObject> StoreItemStockObjects { get; set; }
    }
}
