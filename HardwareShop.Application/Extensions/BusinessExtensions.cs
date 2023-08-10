using HardwareShop.Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareShop.Application.Extensions
{
  
   
    public static class BusinessExtensions
    {
        public static IServiceCollection ConfigureApplication(this IServiceCollection services)
        {
            services.ConfigureRepository();
            return services;
        }
    }
}
