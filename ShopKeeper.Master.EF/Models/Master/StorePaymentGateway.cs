//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopKeeper.Master.EF.Models.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class StorePaymentGateway
    {
        public long StorePaymentgatewayId { get; set; }
        public long PaymentGatewayId { get; set; }
        public long StoreId { get; set; }
        public string MerchantId { get; set; }
    
        public virtual PaymentGateway PaymentGateway { get; set; }
        public virtual Store Store { get; set; }
    }
}