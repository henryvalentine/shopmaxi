//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopkeeperStore.EF.Models.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChildMenu
    {
        public ChildMenu()
        {
            this.ChildMenuRoles = new HashSet<ChildMenuRole>();
        }
    
        public int ChildMenuId { get; set; }
        public int ParentMenuId { get; set; }
        public bool IsParent { get; set; }
        public Nullable<int> ParentChildId { get; set; }
        public string Value { get; set; }
        public string Href { get; set; }
        public int ChildMenuOrder { get; set; }
    
        public virtual ParentMenu ParentMenu { get; set; }
        public virtual ICollection<ChildMenuRole> ChildMenuRoles { get; set; }
    }
}
