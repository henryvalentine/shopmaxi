
using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System.Collections.Generic;
    
    public partial  class UserProfileObject
    {
        public long Id { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string PhotofilePath { get; set; }
        public bool IsActive { get; set; }
        public string ContactEmail { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeLine { get; set; }

        public virtual ICollection<AspNetUserObject> AspNetUserObjects { get; set; }
        public virtual List<CustomerObject> CustomerObjects { get; set; }
        public virtual ICollection<EmployeeObject> EmployeeObjects { get; set; }
        public virtual ICollection<EmployeeObject> EmployeesObject1 { get; set; }
        public virtual ICollection<PersonContactObject> PersonContactObjects { get; set; }
        public virtual ICollection<StoreContactObject> StoreContactObjects { get; set; }
        public virtual ICollection<StoreMessageObject> StoreMessageObjects { get; set; }
    }
}

