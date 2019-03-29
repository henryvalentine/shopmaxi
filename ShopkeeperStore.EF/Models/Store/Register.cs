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
    
    public partial class Register
    {
        public Register()
        {
            this.Sales = new HashSet<Sale>();
        }
    
        public int RegisterId { get; set; }
        public string Name { get; set; }
        public int CurrentOutletId { get; set; }
        public string HostName { get; set; }
        public string IpAddress { get; set; }
    
        public virtual StoreOutlet StoreOutlet { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
