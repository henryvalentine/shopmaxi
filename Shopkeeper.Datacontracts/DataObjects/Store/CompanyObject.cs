
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompanyObject
    {
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string RcNumber { get; set; }
        public Nullable<int> TotalEmployee { get; set; }
        public string Description { get; set; }
    }
}
