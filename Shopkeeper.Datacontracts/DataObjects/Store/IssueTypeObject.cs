
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class IssueTypeObject
    {
        public int IssueTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<ReturnedProductObject> ReturnedProductObjects { get; set; }
        public virtual ICollection<StoreItemIssueObject> StoreItemIssueObjects { get; set; }
    }
}
