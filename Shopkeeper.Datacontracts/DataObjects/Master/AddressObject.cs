
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class AddressObject
    {
        public long AddressId { get; set; }
        public string StreetNo { get; set; }
        public long CityId { get; set; }

        public virtual CityObject CityObject { get; set; }
    }
}
