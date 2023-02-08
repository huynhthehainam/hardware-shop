using HardwareShop.Business.Dtos;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface IUserService
    {
        Task<CreatedUserDto> CreateUserAsync(string username, string password);
        Task<List<UserDto>> GetUserDtosAsync();
        Task<LoginDto?> LoginAsync(string username, string password);
        Task<LoginDto?> LoginByTokenAsync(string token);
        Task<IAssetTable?> GetCurrentUserAvatarAsync();
        Task<PageData<UserDto>?> GetUsersOfShopAsync(PagingModel pagingModel, string? search);

    }
}
