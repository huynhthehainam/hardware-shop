using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareShop.Business.Extensions
{
    public static class BusinessExtension
    {
        public static void ConfigureBusiness(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ICustomerDebtService, CustomerDebtService>();
        }
    }
}
