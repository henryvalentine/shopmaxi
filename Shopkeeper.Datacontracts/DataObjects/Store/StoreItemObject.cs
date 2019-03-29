
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class StoreItemObject
    {
        public long StoreItemId { get; set; }
        public long StoreItemBrandId { get; set; }
        public long StoreItemTypeId { get; set; }
        public long StoreItemCategoryId { get; set; }
        public Nullable<int> ChartOfAccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TechSpechs { get; set; }
        public Nullable<long> ParentItemId { get; set; }

        public virtual ChartOfAccountObject ChartOfAccountObject { get; set; }
        public virtual StoreItemBrandObject StoreItemBrandObject { get; set; }
        public virtual StoreItemCategoryObject StoreItemCategoryObject { get; set; }
        public virtual StoreItemTypeObject StoreItemTypeObject { get; set; }
        public virtual ICollection<StoreItemStockObject> StoreItemStockObjects { get; set; }
        public virtual ICollection<StoreItemSupplierObject> StoreItemSupplierObjects { get; set; }
        public virtual ICollection<WayBillItemObject> WayBillItemObjects { get; set; }

    }
}

