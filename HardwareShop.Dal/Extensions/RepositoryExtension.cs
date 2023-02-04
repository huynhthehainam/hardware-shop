using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
