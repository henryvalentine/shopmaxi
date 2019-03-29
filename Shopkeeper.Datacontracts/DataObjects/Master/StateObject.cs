
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class StateObject
    {
        public long StateId { get; set; }
        public string Name { get; set; }
        public long CountryId { get; set; }

        public virtual ICollection<CityObject> CityObjects { get; set; }
        public virtual CountryObject CountryObject { get; set; }
    }
}
