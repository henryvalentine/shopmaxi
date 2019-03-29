using System.Collections.Generic;
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreAddressObject
    {
        public long StoreAddressId { get; set; }
        public string StreetNo { get; set; }
        public long StoreCityId { get; set; }

        public virtual ICollection<EmployeeObject> EmployeeObjects { get; set; }
        public virtual StoreCityObject StoreCityObject { get; set; }
        public virtual ICollection<StoreOutletObject> StoreOutletObjects { get; set; }
        public virtual ICollection<SupplierAddressObject> SupplierAddressObjects { get; set; }
        public virtual ICollection<WarehouseObject> WarehouseObjects { get; set; }
    }
}
