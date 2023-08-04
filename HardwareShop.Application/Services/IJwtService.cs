using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IJwtService
    {
        CacheUser? GetUserFromToken(string token);
        LoginResponse GenerateTokens(CacheUser cacheUser);
    }
}
