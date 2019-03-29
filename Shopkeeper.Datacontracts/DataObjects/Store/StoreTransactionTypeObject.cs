
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public  class StoreTransactionTypeObject
    {
        public int StoreTransactionTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
    
        public virtual ICollection<StoreTransactionObject> StoreTransactionObjects { get; set; }
    }
}
