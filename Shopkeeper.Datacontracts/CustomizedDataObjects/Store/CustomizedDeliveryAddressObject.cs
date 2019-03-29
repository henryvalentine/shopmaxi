
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class DeliveryAddressObject
    {
        public int PaymentTypeId { get; set; }
        public long ShoppingCartId { get; set; }
        public long StateId { get; set; }
        public long CountryId { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string CouponCode { get; set; }
    }
}

