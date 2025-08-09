using HardwareShop.Application.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IJwtService
    {
        ApplicationUserDto? GetUserFromToken(string token);
        TokenDto GenerateTokens(ApplicationUserDto cacheUser);
    }
}
