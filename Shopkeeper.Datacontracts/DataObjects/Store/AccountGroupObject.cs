
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class AccountGroupObject
    {
        public int AccountGroupId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ChartOfAccountObject> ChartOfAccountObjects { get; set; }
    }
}
