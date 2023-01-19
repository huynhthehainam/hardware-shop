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
            services.AddScoped<IRepository<Account>, RepositoryBase<Account>>();
        }
    }
}
