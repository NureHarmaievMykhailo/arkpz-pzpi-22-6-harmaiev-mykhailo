using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Models;

namespace RoadMonitoringSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Визначаємо таблиці (DbSet)
        public DbSet<RoadSection> RoadSections { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorData> SensorData { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зв’язків між таблицями

            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.RoadSection)
                .WithMany(r => r.Sensors)
                .HasForeignKey(s => s.RoadSectionID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SensorData>()
                .HasOne(sd => sd.Sensor)
                .WithMany(s => s.SensorData)
                .HasForeignKey(sd => sd.SensorID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.RoadSection)
                .WithMany(r => r.Alerts)
                .HasForeignKey(a => a.RoadSectionID)
                .OnDelete(DeleteBehavior.Cascade);

            // Унікальність імені користувача
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    }
}
