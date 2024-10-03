using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;

namespace SignalRAssignment.DataContext
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderID, od.ProductID });
            // Account 1-to-Many Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountID);

            // Category 1-to-Many Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);

            // Supplier 1-to-Many Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierID);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order);

            modelBuilder.Entity<Product>()
                .HasMany(p=>p.OrderDetails)
                .WithOne(od => od.Product);  

            modelBuilder.Entity<Account>().ToTable(nameof(Accounts));
            modelBuilder.Entity<Category>().ToTable(nameof(Categories));
            modelBuilder.Entity<Supplier>().ToTable(nameof(Suppliers));
            modelBuilder.Entity<Product>().ToTable(nameof(Products));
            modelBuilder.Entity<Order>().ToTable(nameof(Orders));
            modelBuilder.Entity<OrderDetail>().ToTable(nameof(OrderDetails));
        }

    }
}
