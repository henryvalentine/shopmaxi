
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeliveryAddressObject
    {
        public long Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public Nullable<long> CustomerId { get; set; }
        public string CustomerIpAddress { get; set; }
        public string ZipCode { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string ContactEmail { get; set; }

        public virtual CustomerObject CustomerObject { get; set; }
        public virtual StoreCityObject StoreCityObject { get; set; }
        public virtual ICollection<ShoppingCartObject> ShoppingCartObjects { get; set; }
    }
}






