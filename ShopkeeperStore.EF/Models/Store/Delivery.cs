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
    
    public partial class Delivery
    {
        public long DeliveryId { get; set; }
        public long PurchaseOrderId { get; set; }
        public System.DateTime ExpectedDeliveryDate { get; set; }
        public Nullable<System.DateTime> AcutalDeliveryDate { get; set; }
        public long ReceivedById { get; set; }
    }
}
