
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public  class WarehouseObject
    {
        public long WarehouseId { get; set; }
        public string Name { get; set; }
        public long AddressId { get; set; }
        public long CompanyId { get; set; }
    
        public virtual StoreAddressObject AddressObject { get; set; }
    }
}
