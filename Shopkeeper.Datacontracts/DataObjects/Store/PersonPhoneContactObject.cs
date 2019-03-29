
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public  class UserProfilePhoneContactObject
    {
        public long UserProfilePhoneContactId { get; set; }
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPrimaryNumber { get; set; }
        public string PhoneTag { get; set; }
    
        public virtual UserProfileObject UserProfileObject { get; set; }
    }
}
