using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Extensions
{
    public static class BusinessExtension
    {
        public static void ConfigureBusiness(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IShopService, ShopService>();

        }
    }
}
