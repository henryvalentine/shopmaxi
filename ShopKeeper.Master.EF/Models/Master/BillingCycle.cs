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
    
    public partial class BillingCycle
    {
        public BillingCycle()
        {
            this.PackagePricings = new HashSet<PackagePricing>();
        }
    
        public long BillingCycleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Duration { get; set; }
        public string Remark { get; set; }
        public Nullable<double> PercentageDiscount { get; set; }
    
        public virtual ICollection<PackagePricing> PackagePricings { get; set; }
    }
}