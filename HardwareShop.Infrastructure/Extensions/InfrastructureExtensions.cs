using System;
using HardwareShop.Application;
using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Application.CQRS.OrderArea.Interfaces;
using HardwareShop.Application.CQRS.ProductArea.Interfaces;
using HardwareShop.Application.CQRS.ShopArea.Interfaces;
using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Application.CQRS.WarehouseArea.Interfaces;
using HardwareShop.Application.Services;
using HardwareShop.Domain;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Data.Repositories;
using HardwareShop.Infrastructure.Kafka;
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

            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IUnitCategoryService, UnitCategoryService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IMinioService, MinioService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICurrentUserService, WebCurrentUserService>();
            services.AddSingleton<IHashingPasswordService, HashingPasswordService>();
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            services.AddScoped<ITestService, TestService>();

            #region Repositories
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IShopRepository, ShopRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            #endregion
            return services;
        }
    }
}
