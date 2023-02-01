using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
namespace HardwareShop.Dal
{
    public class MainDatabaseContext : DbContext
    {
        public MainDatabaseContext(
            DbContextOptions<MainDatabaseContext> options) : base(options)
        {
        }
        public DbSet<UserAsset> UserAssets => Set<UserAsset>();
        public DbSet<UserShop> UserShops => Set<UserShop>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Shop> Shops => Set<Shop>();
        public DbSet<ShopAsset> ShopAssets => Set<ShopAsset>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<UnitCategory> UnitCategories => Set<UnitCategory>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductAsset> ProductAssets => Set<ProductAsset>();
        public DbSet<CustomerDebt> CustomerDebts => Set<CustomerDebt>();
        public DbSet<CustomerDebtHistory> CustomerDebtHistories => Set<CustomerDebtHistory>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<WarehouseProduct> WarehouseProducts => Set<WarehouseProduct>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            UserAsset.BuildModel(modelBuilder);
            UserShop.BuildModel(modelBuilder);
            Customer.BuildModel(modelBuilder);
            CustomerDebt.BuildModel(modelBuilder);
            CustomerDebtHistory.BuildModel(modelBuilder);
            Invoice.BuildModel(modelBuilder);
            InvoiceDetail.BuildModel(modelBuilder);
            Product.BuildModel(modelBuilder);
            ProductAsset.BuildModel(modelBuilder);
            ProductCategory.BuildModel(modelBuilder);
            Shop.BuildModel(modelBuilder);
            ShopAsset.BuildModel(modelBuilder);
            Unit.BuildModel(modelBuilder);
            UnitCategory.BuildModel(modelBuilder);
            Warehouse.BuildModel(modelBuilder);
            User.BuildModel(modelBuilder);
            WarehouseProduct.BuildModel(modelBuilder);

        }
    }
}