using Microsoft.EntityFrameworkCore;
using SafeVaultProject.Models;

namespace SafeVaultProject.Data
{
    public class SafeVaultContext : DbContext
    {
        public SafeVaultContext(DbContextOptions<SafeVaultContext> opts)
            : base(opts) { }

        public DbSet<User> Users { get; set; }
    }
}
