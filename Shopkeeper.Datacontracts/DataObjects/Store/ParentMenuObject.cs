
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class ParentMenuObject
    {
        public int ParentMenuId { get; set; }
        public string Value { get; set; }
        public string Href { get; set; }
        public int MenuOrder { get; set; }
        public string GlyphIconClass { get; set; }

        public virtual List<ChildMenuObject> ChildMenuObjects { get; set; }
        public virtual List<ParentMenuRoleObject> ParentMenuRoleObjects { get; set; }
    }
}

