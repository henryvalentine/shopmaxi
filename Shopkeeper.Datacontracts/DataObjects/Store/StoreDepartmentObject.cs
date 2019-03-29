
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public  class StoreDepartmentObject
    {
        public long StoreDepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<EmployeeObject> EmployeeObjects { get; set; }
    }
}
