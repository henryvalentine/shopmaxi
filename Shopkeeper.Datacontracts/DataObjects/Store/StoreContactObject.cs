
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreContactObject
    {
        public long StoreContactId { get; set; }
        public long Id { get; set; }
        public long StoreDepartmentId { get; set; }
        public string DateCreated { get; set; }

        public virtual UserProfileObject UserProfile { get; set; }
        public virtual StoreDepartmentObject StoreDepartmentObject { get; set; }
    }
}
