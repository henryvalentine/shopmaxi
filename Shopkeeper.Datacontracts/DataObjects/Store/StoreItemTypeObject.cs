
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoreItemTypeObject
    {
        public long StoreItemTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SampleImagePath { get; set; }

        public virtual ICollection<StoreItemObject> StoreItemObjects { get; set; }
    }
}
