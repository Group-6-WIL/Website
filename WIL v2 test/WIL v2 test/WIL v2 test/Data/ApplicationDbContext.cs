using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WIL_v2_test.Models;

namespace WIL_v2_test.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Events> Events { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Specify precision and scale for the Amount property
            modelBuilder.Entity<Donation>()
                .Property(d => d.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
