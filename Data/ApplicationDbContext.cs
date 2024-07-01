

using POS_OS_GG.Models;

namespace POS_OS_GG.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasIndex(x => x.CompanyId).IsUnique();
            builder.Entity<Order>().Property(x=>x.UserDeliveredId).IsRequired(false);

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                 .HasIndex(x => x.CompanyId)
                 .IsUnique();

            // Configure Product relationships
            builder.Entity<Product>()
                .HasOne(p => p.ApplicationUser)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UserRegistratedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Order relationships
            builder.Entity<Order>()
                .HasOne(o => o.UserOrdered)
                .WithMany(u => u.OrdersMade)
                .HasForeignKey(o => o.UserOrderedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.UserDelivered)
                .WithMany(u => u.OrdersDelivered)
                .HasForeignKey(o => o.UserDeliveredId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure OrderProduct relationships
            builder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            builder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.ProductId);


        }
    }
}
