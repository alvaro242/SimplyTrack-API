using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimplyTrack.Api.Models;

namespace ApplicationDBContext.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Exercise> Exercises { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<SetEntity> Sets { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Exercise -> User relationship
            builder.Entity<Exercise>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Exercise>()
                .HasMany(e => e.Sessions)
                .WithOne(s => s.Exercise)
                .HasForeignKey(s => s.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Session>()
                .HasMany(s => s.Sets)
                .WithOne(st => st.Session)
                .HasForeignKey(st => st.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();
        }
    }
}