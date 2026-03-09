using HardwareShop.Application.Dtos;

namespace HardwareShop.Application.Services
{
    public interface IJwtService
    {
        ApplicationUserDto? GetUserFromToken(string token);
    }
}
