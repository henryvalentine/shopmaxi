
using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreOutletObject
    {
        public string Address { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string StoreLogoPath { get; set; }
        public string StateName { get; set; }
        public long StoreCityId { get; set; }
        public long StoreStateId { get; set; }
        public long StoreCountryId { get; set; }
        //public StoreSettingObject StoreSettingObject { get; set; }
         
    }
}

