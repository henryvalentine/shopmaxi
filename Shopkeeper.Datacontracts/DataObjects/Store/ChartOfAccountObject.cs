
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class ChartOfAccountObject
    {
        public int ChartOfAccountId { get; set; }
        public int AccountGroupId { get; set; }
        public string AccountType { get; set; }
        public string AccountCode { get; set; }

        public virtual AccountGroupObject AccountGroupObject { get; set; }
        public virtual ICollection<StoreItemObject> StoreItemObjects { get; set; }
    }
}


