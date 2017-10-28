using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication.Models.CodeFirst;
using System.Collections.Generic;

namespace WebApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public virtual ICollection<Label> Labels { get; set; }
        public virtual ICollection<WebApplication.Models.CodeFirst.Task> Tasks { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WebApplication.Models.CodeFirst.Label> Labels { get; set; }
        public System.Data.Entity.DbSet<WebApplication.Models.CodeFirst.Task> Tasks { get; set; }
        public System.Data.Entity.DbSet<WebApplication.Models.CodeFirst.Reminder> Reminds { get; set; }
        public System.Data.Entity.DbSet<WebApplication.Models.CodeFirst.TaskOccurrence> DailyRepetitions { get; set; }
        public System.Data.Entity.DbSet<WebApplication.Models.CodeFirst.Visitor> Visitors { get; set; }
        public System.Data.Entity.DbSet<WebApplication.Models.CodeFirst.VisitorBot> VisitorBots { get; set; }
    }
}