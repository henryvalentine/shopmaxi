
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class PersonContactObject
    {
        public long UserProfileContactId { get; set; }
        public long UserId { get; set; }
        public int ContactTypeId { get; set; }
        public string ContactValue { get; set; }
        public bool IsDefault { get; set; }
        public string ContactTagName { get; set; }

        public virtual ContactTypeObject ContactTypeObject { get; set; }
        public virtual UserProfileObject UserProfileObject { get; set; }
    }
}
