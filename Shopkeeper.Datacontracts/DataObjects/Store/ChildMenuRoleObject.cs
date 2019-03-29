
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ChildMenuRoleObject
    {
        public int ChildMenuRoleId { get; set; }
        public int ChildMenuId { get; set; }
        public string RoleId { get; set; }

        public virtual ChildMenuObject ChildMenu { get; set; }
    }

    public partial class ChildMenuRoleObject
    {
        public string RoleName { get; set; }

    }
}

