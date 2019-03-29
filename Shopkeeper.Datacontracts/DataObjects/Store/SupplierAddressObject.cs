
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    
    public partial class SupplierAddressObject
    {
        public long SupplierAddressId { get; set; }
        public long SupplierId { get; set; }
        public long AddressId { get; set; }
        public System.DateTime DateFrom { get; set; }
        public System.DateTime DateTo { get; set; }
        public int AddressTypeId { get; set; }

        public virtual StoreAddressObject AddressObject { get; set; }
        public virtual AddressTypeObject AddressTypeObject { get; set; }
        public virtual SupplierObject SupplierObject { get; set; }
    }
}
