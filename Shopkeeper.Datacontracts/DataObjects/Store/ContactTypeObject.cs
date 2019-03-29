
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System.Collections.Generic;
    
    public partial class ContactTypeObject
    {
        public int ContactTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PersonContactObject> PersonContactObject { get; set; }
    }
}
