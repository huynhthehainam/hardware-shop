

namespace HardwareShop.Application.Services
{
    public interface ISeedingService
    {
        Task SeedDataAsync(bool isDevelopment);
    }
}