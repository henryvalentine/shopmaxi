
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoreItemBrandObject
    {

        public long StoreItemBrandId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
    
        public virtual ICollection<StoreItemObject> StoreItems { get; set; }
    }
}
