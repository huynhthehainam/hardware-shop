
namespace HardwareShop.Application.Services
{
    public interface ITestService
    {
        public Task<int> TestEntityAsync();
        public Task<List<string?>> TestEncryptedAsync(CancellationToken cancellationToken);
    }
}