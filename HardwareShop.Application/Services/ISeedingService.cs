

namespace HardwareShop.Application.Services
{
    public interface ISeedingService
    {
        Task SeedDataAsync(string firstUserId);
        Task EnsureClientExistsAsync(string realm);
        Task<string> EnsureUserExistsAsync(string realm);
        Task EnsureRealmExistsAsync(string realm);
        Task EnsureKafkaTopicsExistAsync();
    }
}