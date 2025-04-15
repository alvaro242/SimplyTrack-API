using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimplyTrack_API.Models;

namespace SimplyTrack_API.Data
{
    public class AppDbContext : IdentityDbContext<User> // Inherit from IdentityDbContext with your custom User model
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define your database tables (DbSets)
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WeightProgression> WeightProgressions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure Identity configurations are applied

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<Workout>()
                .HasOne(w => w.User)
                .WithMany(u => u.Workouts)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Workout)
                .WithMany(w => w.Exercises)
                .HasForeignKey(e => e.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WeightProgression>()
                .HasOne(wp => wp.Exercise)
                .WithMany(e => e.WeightProgressions)
                .HasForeignKey(wp => wp.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<Workout>()
                .HasIndex(w => w.UserId);

            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.WorkoutId);

            modelBuilder.Entity<WeightProgression>()
                .HasIndex(wp => wp.ExerciseId);
        }
    }
}