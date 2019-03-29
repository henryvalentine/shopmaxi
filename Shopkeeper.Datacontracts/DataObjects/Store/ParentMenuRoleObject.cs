
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ParentMenuRoleObject
    {
        public int ParentMenuRoleId { get; set; }
        public int ParentMenuId { get; set; }
        public string RoleId { get; set; }

        public virtual ParentMenuObject ParentMenu { get; set; }
    }
}
