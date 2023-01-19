using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
namespace HardwareShop.Dal
{
    public class MainDatabaseContext : DbContext
    {
        public MainDatabaseContext(
            DbContextOptions<MainDatabaseContext> options) : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Shop> Shops => Set<Shop>();
        public DbSet<ShopAsset> ShopAssets => Set<ShopAsset>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<AccountShop> AccountShops => Set<AccountShop>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<UnitCategory> UnitCategories => Set<UnitCategory>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<CustomerDebt> CustomerDebts => Set<CustomerDebt>();
        public DbSet<CustomerDebtHistory> CustomerDebtHistories => Set<CustomerDebtHistory>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitCategory>(entity =>
            {
                entity.HasKey(entity => entity.Id);
            });
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(entity => entity.Id);
                entity.HasOne(e => e.UnitCategory).WithMany(e => e.Units).HasForeignKey(e => e.UnitCategoryId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Shop>(s =>
            {
                s.HasKey(x => x.Id);
            });
            modelBuilder.Entity<ShopAsset>(s =>
            {
                s.HasKey(s => s.ShopId);
                s.HasOne(e => e.Shop).WithOne(e => e.ShopAsset).HasForeignKey<ShopAsset>(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Account>(s =>
            {
                s.HasKey(a => a.Id);
            });
            modelBuilder.Entity<AccountShop>(e =>
            {
                e.HasKey(e => e.AccountId);
                e.HasOne(s => s.Account).WithOne(e => e.ShopAccount).HasForeignKey<AccountShop>(e => e.AccountId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(s => s.Shop).WithMany(e => e.ShopAccounts).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ProductCategory>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Shop).WithMany(e => e.ProductCategories).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Warehouse>(w =>
            {
                w.HasKey(e => e.Id);
                w.HasOne(e => e.Shop).WithMany(e => e.Warehouses).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Unit).WithMany(e => e.Products).HasForeignKey(e => e.UnitId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.Shop).WithMany(e => e.Products).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CustomerDebt>(e =>
            {
                e.HasKey(e => e.CustomerId);
                e.HasOne(e => e.Customer).WithOne(e => e.Debt).HasForeignKey<CustomerDebt>(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CustomerDebtHistory>(h =>
            {
                h.HasKey(e => e.Id);
                h.HasOne(e => e.CustomerDebt).WithMany(e => e.Histories).HasForeignKey(e => e.CustomerDebtId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}