
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class TransactionTypeObject
    {
        public int TransactionTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
    
        public virtual ICollection<TransactionObject> TransactionObjects { get; set; }
    }
}
