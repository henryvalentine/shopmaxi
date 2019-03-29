using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ShopKeeper.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual ApplicationDbContext.UserProfile UserInfo { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(string connectionString) : base(connectionString)
        {
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
            _conn = connectionString;
        }
       
        private static string _conn = string.Empty;
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext(_conn);
        }

        public System.Data.Entity.DbSet<UserProfile> UserInfo { get; set; }
        public class UserProfile
        {
            public long Id { get; set; }
            public string OtherNames { get; set; }
            public string LastName { get; set; }
            public bool IsActive { get; set; }
            public string ContactEmail { get; set; }
            public string MobileNumber { get; set; }
            public string OfficeLine { get; set; }
            public DateTime? Birthday { get; set; }
            public string Gender { get; set; }
            public string PhotofilePath { get; set; }
            public int SalutationId { get; set; }
        }
    }
}

