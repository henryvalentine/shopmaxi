
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class AddressTypeObject
    {
        public int AddressTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<SupplierAddressObject> SupplierAddressObjects { get; set; }
    }
}
