namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;

    public partial class AspNetUserObject
    {

        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public Nullable<long> UserInfo_Id { get; set; }

        public virtual ICollection<AspNetUserClaimObject> AspNetUserClaimObjects { get; set; }
        public virtual ICollection<AspNetUserLoginObject> AspNetUserLoginObjects { get; set; }
        public virtual UserProfileObject UserProfileObject { get; set; }
        public virtual ICollection<AspNetRoleObject> AspNetRoleObjects { get; set; }
    }
}
