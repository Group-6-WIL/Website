using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WIL_Web.Models;

namespace WIL_Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<AdminDashBoard> AdminDashBoards { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<ContactUs> Contactus { get; set; }
        public DbSet<Locations> Locations { get; set; }
        public DbSet<Donations> Donations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
