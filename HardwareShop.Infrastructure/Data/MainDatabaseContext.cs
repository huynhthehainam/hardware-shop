using System.Reflection;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace HardwareShop.Infrastructure.Data
{
    public class MainDatabaseContext : DbContext
    {
        public MainDatabaseContext(
            DbContextOptions<MainDatabaseContext> options) : base(options)
        {
        }
        public DbSet<Asset> Assets => Set<Asset>();
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
        public DbSet<ProductCategoryProduct> ProductCategoryProducts => Set<ProductCategoryProduct>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<CountryAsset> CountryAssets => Set<CountryAsset>();
        public DbSet<ShopPhone> ShopPhones => Set<ShopPhone>();
        public DbSet<ShopSetting> ShopSettings => Set<ShopSetting>();
        public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
        public DbSet<ChatSessionMember> ChatSessionMembers => Set<ChatSessionMember>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<ChatMessageStatus> ChatMessageStatuses => Set<ChatMessageStatus>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}