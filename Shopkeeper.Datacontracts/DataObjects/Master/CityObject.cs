
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class CityObject
    {
        public long CityId { get; set; }
        public string Name { get; set; }
        public long StateId { get; set; }

        public virtual ICollection<AddressObject> AddressObjects { get; set; }
        public virtual StateObject StateObject { get; set; }
    }
}
