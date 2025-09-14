using Microsoft.EntityFrameworkCore;

namespace Wyvern.Database.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Waitlist> Waitlist { get; set; }
    }
}
