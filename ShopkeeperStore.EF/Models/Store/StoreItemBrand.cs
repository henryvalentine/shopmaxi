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
    
    public partial class StoreItemBrand
    {
        public StoreItemBrand()
        {
            this.StoreItems = new HashSet<StoreItem>();
        }
    
        public long StoreItemBrandId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime LastUpdated { get; set; }
    
        public virtual ICollection<StoreItem> StoreItems { get; set; }
    }
}