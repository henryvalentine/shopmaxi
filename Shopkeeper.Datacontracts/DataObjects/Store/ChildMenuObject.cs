
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;

    public partial class ChildMenuObject
    {

        public int ChildMenuId { get; set; }
        public int ParentMenuId { get; set; }
        public bool IsParent { get; set; }
        public Nullable<int> ParentChildId { get; set; }
        public string Value { get; set; }
        public string Href { get; set; }
        public int ChildMenuOrder { get; set; }

        public virtual ParentMenuObject ParentMenu { get; set; }
        public virtual List<ChildMenuRoleObject> ChildMenuRoleObjects { get; set; }
    }
}
