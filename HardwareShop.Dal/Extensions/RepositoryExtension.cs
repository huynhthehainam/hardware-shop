using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
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
        }
    }
}
