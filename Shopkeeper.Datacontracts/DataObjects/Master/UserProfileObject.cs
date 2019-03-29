
using System;

namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System.Collections.Generic;
    
    public partial  class UserProfileObject
    {
        public long Id { get; set; }
        public Nullable<int> SalutationId { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string OtherNames { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string PhotofilePath { get; set; }
        public bool IsActive { get; set; }
        public string ContactEmail { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeLine { get; set; }

        public virtual ICollection<AspNetUserObject> AspNetUserObjects { get; set; }
        public virtual ICollection<MessageObject> MessageObjects { get; set; }
        public virtual SalutationObject SalutationObject { get; set; }
        public virtual ICollection<StoreContactObject> StoreContactObjects { get; set; }
    }
}

