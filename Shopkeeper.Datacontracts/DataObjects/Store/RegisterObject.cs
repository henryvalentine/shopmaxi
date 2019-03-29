
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class RegisterObject
    {
        public int RegisterId { get; set; }
        public string Name { get; set; }
        public int CurrentOutletId { get; set; }

        public virtual StoreOutletObject StoreOutletObject { get; set; }
    }
}
