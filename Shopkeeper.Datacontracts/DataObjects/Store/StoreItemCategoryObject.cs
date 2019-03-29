
using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System.Collections.Generic;
    
    public partial class StoreItemCategoryObject
    {
        public long StoreItemCategoryId { get; set; }
        public Nullable<long> ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public System.DateTime LastUpdated { get; set; }
        
        public virtual ICollection<StoreItemObject> StoreItemObjects { get; set; }
    }
}
