using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Core.Entities;

namespace SupplyChainSystem.Infrastructure.Data
{
    public class SupplyChainDbContext : DbContext
    {
        public SupplyChainDbContext(DbContextOptions<SupplyChainDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<FulfillmentOrder> FulfillmentOrders { get; set; }
        public DbSet<FulfillmentOrderItem> FulfillmentOrderItems { get; set; }
        public DbSet<StockAuditLog> StockAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<InventoryItem>()
                .HasIndex(i => new { i.ProductId, i.WarehouseId })
                .IsUnique();
                
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(p => p.Sku).IsUnique();
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Warehouse Manager" },
                new Role { Id = 3, Name = "Staff" });
        }
    }
}
