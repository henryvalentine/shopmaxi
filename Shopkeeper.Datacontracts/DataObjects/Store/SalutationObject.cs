
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class SalutationObject
    {
        public int SalutationId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserProfileObject> UserProfilesObject { get; set; }
    }
}
