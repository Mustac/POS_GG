

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasIndex(x => x.CompanyId).IsUnique();

            base.OnModelCreating(builder);

            // Configure the relationships
            builder.Entity<Product>()
                .HasOne(p => p.ApplicationUser)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UserRegistratedId)
                .HasPrincipalKey(u => u.Id);

            builder.Entity<Order>()
                .HasOne(o => o.UserOrdered)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserOrderedId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(u => u.Id);

            builder.Entity<Order>()
                .HasOne(o => o.UserDelivered)
                .WithMany()
                .HasForeignKey(o => o.UserDeliveredId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(u => u.Id);

            builder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId)
                .HasPrincipalKey(o => o.Id);

            builder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId)
                .HasPrincipalKey(p => p.Id);
        }
    }
}
