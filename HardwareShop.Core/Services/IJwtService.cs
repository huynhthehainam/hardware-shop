using HardwareShop.Core.Models;

namespace HardwareShop.Core.Services
{
    public interface IJwtService
    {
        CacheUser? GetUserFromToken(string token);
        LoginResponse? GenerateTokens(CacheUser cacheUser);
    }
}
