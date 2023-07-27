using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareShop.Domain.Extensions
{
    public static class RepositoryExtension
    {
        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddScoped<DbContext, MainDatabaseContext>();
        }
    }
}
