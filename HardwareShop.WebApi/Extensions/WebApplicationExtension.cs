using HardwareShop.Application.Services;

namespace HardwareShop.WebApi.Extensions
{
    public static class WebApplicationExtension
    {
        public static void SeedData(this WebApplication app)
        {
            IServiceProvider services = app.Services;
            ISeedingService seedingService = services.CreateScope().ServiceProvider.GetRequiredService<ISeedingService>();
            seedingService.SeedData(app.Environment.IsDevelopment());
        }
    }
}