using System.Text.Json;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IUserService
    {
        Task<CreatedUserDto> CreateUserAsync(string username, string password);
        Task<LoginDto?> LoginAsync(string username, string password);
        Task<LoginDto?> LoginByTokenAsync(string token);
        Task<ApplicationResponse<CachedAssetDto>> GetCurrentUserAvatarAsync();
        Task<ApplicationResponse<PageData<UserDto>>> GetUserPageDataOfShopAsync(PagingModel pagingModel, string? search);
        Task<PageData<UserDto>> GetUserPageDataAsync(PagingModel pagingModel, string? search);
        Task<ApplicationResponse> UpdateCurrentUserInterfaceSettings(JsonDocument settings);
        Task<ApplicationResponse<PageData<NotificationDto>>> GetNotificationDtoPageDataOfCurrentUserAsync(PagingModel pagingModel);
        Task<CreatedNotificationDto?> CreateNotificationOfCurrentUserAsync(string? message, string variant, string? translation, JsonDocument? translationParams);
        Task<ApplicationResponse> DismissNotificationOfCurrentUserAsync(Guid id);
        Task<ApplicationResponse> DismissAllNotificationsOfCurrentUserAsync();
        Task<ApplicationResponse> UpdateCurrentUserPasswordAsync(string oldPassword, string newPassword);
    }
}
