
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class StoreContactObject
    {
        public long StoreContactId { get; set; }
        public long Id { get; set; }
        public int ContactTagId { get; set; }
        public string Contact { get; set; }
        public System.DateTime LastUpdated { get; set; }

        public virtual ContactTagObject ContactTagObject { get; set; }
        public virtual PersonObject PersonObject { get; set; }
    }
}
