
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;

    public partial class PersonObject
    {
        public long PersonId { get; set; }
        public string Salutation { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string Gender { get; set; }
        public System.DateTime Birthday { get; set; }
        public string PhotofilePath { get; set; }
        public System.DateTime DateRegistered { get; set; }

        public virtual ICollection<StoreContactObject> StoreContactObjects { get; set; }
    }
}
