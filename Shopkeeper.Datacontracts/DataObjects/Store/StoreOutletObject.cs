
using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System.Collections.Generic;
    
    public partial  class StoreOutletObject
    {
        public int StoreOutletId { get; set; }
        public string OutletName { get; set; }
        public long StoreAddressId { get; set; }  
        public bool IsMainOutlet { get; set; }
        public double? DefaultTax { get; set; }
        public bool IsOperational { get; set; }
        public DateTime DateCreated { get; set; }
        public string FacebookHandle { get; set; }
        public string TwitterHandle { get; set; }
        
        public virtual ICollection<CustomerObject> CustomerObjects { get; set; }
        public virtual ICollection<DailySaleObject> DailySaleObjects { get; set; }
        public virtual ICollection<EmployeeObject> EmployeeObjects { get; set; }
        public virtual ICollection<EmployeeAssigmentObject> EmployeeAssigmentObjects { get; set; }
        public virtual ICollection<PurchaseOrderObject> PurchaseOrderObjects { get; set; }
        public virtual ICollection<PurchaseOrderItemObject> PurchaseOrderItemObjects { get; set; }
        public virtual ICollection<RegisterObject> RegisterObjects { get; set; }
        public virtual ICollection<SaleObject> SaleObjects { get; set; }
        public virtual StoreAddressObject StoreAddressObject { get; set; }
        public virtual ICollection<StoreItemStockObject> StoreItemStocks { get; set; }
        public virtual ICollection<StoreOutletCouponObject> StoreOutletCoupons { get; set; }
        public virtual ICollection<StoreTransactionObject> StoreTransactionObjects { get; set; }
    }
}

