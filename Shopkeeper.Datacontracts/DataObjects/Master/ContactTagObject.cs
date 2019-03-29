
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContactTagObject
    {
        public int ContactTagId { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<StoreContactObject> StoreContactObjects { get; set; }
    }
}
