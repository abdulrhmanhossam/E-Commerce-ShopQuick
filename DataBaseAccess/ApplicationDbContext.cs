using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelClasses;

namespace DataBaseAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }
        public DbSet<UserOrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set;}

    }
}
