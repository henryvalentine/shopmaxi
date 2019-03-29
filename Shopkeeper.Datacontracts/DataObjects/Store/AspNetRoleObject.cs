
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetRoleObject
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AspNetUserObject> AspNetUsers { get; set; }
    }
}
