
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemCategoryObject
    {
        public StoreItemCategoryObject ParentCategoryObject { get; set; }
        public string ParentName { get; set; }
        public List<StoreItemStockObject> StockItems { get; set; }
        public List<StoreItemSoldObject> SoldItems { get; set; }
    }
}
