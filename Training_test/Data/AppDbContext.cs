using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
namespace WebApplication3.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}
