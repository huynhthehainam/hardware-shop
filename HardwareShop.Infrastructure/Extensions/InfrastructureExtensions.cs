using System;
using HardwareShop.Application.Services;
using HardwareShop.Domain;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareShop.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("AppConn");
            services.AddDbContext<MainDatabaseContext>(options =>
                  options.UseSqlServer(connectionString));
            services.AddScoped<DbContext, MainDatabaseContext>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ICustomerDebtService, CustomerDebtService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IUnitCategoryService, UnitCategoryService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICurrentUserService, WebCurrentUserService>();
            services.AddSingleton<IHashingPasswordService, HashingPasswordService>();
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            services.AddScoped<ISeedingService, SeedingService>();
            services.AddScoped<ITestService, TestService>();
            return services;
        }
    }
}