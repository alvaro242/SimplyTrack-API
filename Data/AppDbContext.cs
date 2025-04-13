using Microsoft.EntityFrameworkCore;
using SimplyTrack_API.Models;

namespace SimplyTrack_API.Data
{



    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define your database tables (DbSets)
        public DbSet<User> Users { get; set; }
    }
}