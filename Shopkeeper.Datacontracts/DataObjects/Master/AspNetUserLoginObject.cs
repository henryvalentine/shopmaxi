
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class AspNetUserLoginObject
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUserObject AspNetUser { get; set; }
    }
}
