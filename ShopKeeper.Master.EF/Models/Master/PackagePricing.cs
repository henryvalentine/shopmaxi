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
    
    public partial class PackagePricing
    {
        public long PackagePricingId { get; set; }
        public long SubscriptionPackageId { get; set; }
        public long BillingCycleId { get; set; }
        public double Price { get; set; }
    
        public virtual BillingCycle BillingCycle { get; set; }
        public virtual SubscriptionPackage SubscriptionPackage { get; set; }
    }
}
