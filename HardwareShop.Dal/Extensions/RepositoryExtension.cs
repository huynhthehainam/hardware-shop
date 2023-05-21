using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HardwareShop.Dal.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareShop.Dal.Extensions
{
    public static class RepositoryExtension
    {
        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddScoped<DbContext, MainDatabaseContext>();
            services.AddScoped<IRepository<User>, RepositoryBase<User>>();
            services.AddScoped<IRepository<Shop>, RepositoryBase<Shop>>();
            services.AddScoped<IRepository<Warehouse>, RepositoryBase<Warehouse>>();
            services.AddScoped<IRepository<UserShop>, RepositoryBase<UserShop>>();
            services.AddScoped<IRepository<UserAsset>, RepositoryBase<UserAsset>>();
            services.AddScoped<IRepository<Product>, RepositoryBase<Product>>();
            services.AddScoped<IRepository<WarehouseProduct>, RepositoryBase<WarehouseProduct>>();
            services.AddScoped<IRepository<ProductAsset>, RepositoryBase<ProductAsset>>();
            services.AddScoped<IRepository<ShopAsset>, RepositoryBase<ShopAsset>>();
            services.AddScoped<IRepository<Invoice>, RepositoryBase<Invoice>>();
            services.AddScoped<IRepository<CustomerDebt>, RepositoryBase<CustomerDebt>>();
            services.AddScoped<IRepository<Customer>, RepositoryBase<Customer>>();
            services.AddScoped<IRepository<Order>, RepositoryBase<Order>>();
            services.AddScoped<IRepository<CustomerDebtHistory>, RepositoryBase<CustomerDebtHistory>>();
            services.AddScoped<IRepository<Unit>, RepositoryBase<Unit>>();
            services.AddScoped<IRepository<UnitCategory>, RepositoryBase<UnitCategory>>();
            services.AddScoped<IRepository<ProductCategory>, RepositoryBase<ProductCategory>>();
            services.AddScoped<IRepository<ProductCategoryProduct>, RepositoryBase<ProductCategoryProduct>>();
            services.AddScoped<IRepository<Notification>, RepositoryBase<Notification>>();
            services.AddScoped<IRepository<Country>, RepositoryBase<Country>>();
            services.AddScoped<IRepository<ShopSetting>, RepositoryBase<ShopSetting>>();
            services.AddScoped<IAssetRepository, AssetRepository>();
        }
    }
}
