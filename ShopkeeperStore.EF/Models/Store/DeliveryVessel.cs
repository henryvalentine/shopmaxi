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
    
    public partial class DeliveryVessel
    {
        public DeliveryVessel()
        {
            this.ShoppingCartDeliveries = new HashSet<ShoppingCartDelivery>();
        }
    
        public long DeliveryVesselId { get; set; }
        public string DeliveryTypeCode { get; set; }
        public string RegistrationCode { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<ShoppingCartDelivery> ShoppingCartDeliveries { get; set; }
    }
}