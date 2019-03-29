
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class BillingCycleObject
    {
        public long BillingCycleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Duration { get; set; }
        public string Remark { get; set; }
        public Nullable<double> PercentageDiscount { get; set; }

        public virtual ICollection<PackagePricingObject> PackagePricingObjects { get; set; }
    }
}
