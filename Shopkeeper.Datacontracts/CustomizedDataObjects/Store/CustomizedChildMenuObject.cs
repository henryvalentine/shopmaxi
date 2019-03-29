
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ChildMenuObject
    {
        public virtual List<ChildMenuObject> SecondLevelChildMenuObjects { get; set; }
        public string RoleName { get; set; }

    }
}
