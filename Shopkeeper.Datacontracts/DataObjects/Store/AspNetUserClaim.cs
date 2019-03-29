
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class AspNetUserClaimObject
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    
        public virtual AspNetUserObject AspNetUser { get; set; }
    }
}
