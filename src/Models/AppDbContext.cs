using Microsoft.EntityFrameworkCore;

namespace AccountManagementAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    }
}
