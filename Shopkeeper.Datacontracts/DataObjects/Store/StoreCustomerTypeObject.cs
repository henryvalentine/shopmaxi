
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class StoreCustomerTypeObject
    {
        public int StoreCustomerTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CreditWorthy { get; set; }
        public double CreditLimit { get; set; }

        public virtual ICollection<CustomerObject> CustomerObjects { get; set; }
    }

    public partial class StoreCustomerTypeObject
    {
        public string CreditWorthyStr { get; set; }
        public string CreditLimitStr { get; set; }
    }
}

