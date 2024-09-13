using ABP_Task_Zakharov.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ABP_Task_Zakharov.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування відношень та первинних ключів
            modelBuilder.Entity<ConferenceRoom>()
                .HasMany(c => c.Services)
                .WithMany();

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.SelectedServices)
                .WithMany();
        }
    }
}
