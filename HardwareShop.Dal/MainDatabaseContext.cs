﻿using System.Reflection;
using HardwareShop.Core.Bases;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string configurationNamespace = "HardwareShop.Dal.ModelConfigurations";
            const string buildModelMethodName = "BuildModel";
            List<Type> configurations = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass && e.Namespace == configurationNamespace && (e.BaseType?.GetGenericArguments().FirstOrDefault()?.BaseType == typeof(EntityBase) || e.BaseType?.GetGenericArguments().FirstOrDefault()?.BaseType == typeof(AssetEntityBase))).ToList();
            foreach (Type configuration in configurations)
            {
                object? instance = Activator.CreateInstance(configuration, modelBuilder);
                if (instance != null)
                {
                    MethodInfo? buildMethod = configuration.GetMethod(buildModelMethodName);
                    _ = (buildMethod?.Invoke(instance, null));
                }
            }
        }
    }
}