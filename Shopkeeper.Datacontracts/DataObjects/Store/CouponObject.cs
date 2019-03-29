
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class CouponObject
    {
        public long CouponId { get; set; }
        public string Code { get; set; }
        public double PercentageDeduction { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public double MinimumOrderAmount { get; set; }
        public string Title { get; set; }

        public virtual ICollection<StoreOutletCouponObject> StoreOutletCouponObjects { get; set; }
    }
}
