using Microsoft.EntityFrameworkCore;
using OWMS.Models;

namespace OWMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<IntervalNumber> IntervalNumbers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<Inventory> Inventorys { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}


