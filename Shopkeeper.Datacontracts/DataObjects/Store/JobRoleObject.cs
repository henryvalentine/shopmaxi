
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public  class JobRoleObject
    {
        public long JobRoleId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public string Responsibilities { get; set; }
        public string MinQualification { get; set; }

        public virtual ICollection<EmployeeAssigmentObject> EmployeeAssigmentObjects { get; set; }
    }
}
