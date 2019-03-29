
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoreOutletCouponObject
    {
        public long StoreOutletCouponId { get; set; }
        public long StoreOutletId { get; set; }
        public long CouponId { get; set; }
    
        public virtual CouponObject CouponObject { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
    }
}
