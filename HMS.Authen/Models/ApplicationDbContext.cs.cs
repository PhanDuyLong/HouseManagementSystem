using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authen.Models
{
    public class AuthenDbContext : IdentityDbContext<ApplicationAccount>
    {
        public AuthenDbContext(DbContextOptions<AuthenDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
