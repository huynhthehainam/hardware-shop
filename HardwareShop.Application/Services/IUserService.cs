﻿using System.Text.Json;
using HardwareShop.Application.Dtos;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IUserService
    {
        Task<CreatedUserDto> CreateUserAsync(string username, string password);
        Task<LoginDto?> LoginAsync(string username, string password);
        Task<LoginDto?> LoginByTokenAsync(string token);
        Task<CachedAsset?> GetCurrentUserAvatarAsync();
        Task<PageData<UserDto>?> GetUserPageDataOfShopAsync(PagingModel pagingModel, string? search);
        Task<PageData<UserDto>> GetUserPageDataAsync(PagingModel pagingModel, string? search);
        Task<bool> UpdateCurrentUserInterfaceSettings(JsonDocument settings);
        Task<PageData<NotificationDto>?> GetNotificationDtoPageDataOfCurrentUserAsync(PagingModel pagingModel);
        Task<CreatedNotificationDto?> CreateNotificationOfCurrentUserAsync(string? message, string variant, string? translation, JsonDocument? translationParams);
        Task<bool> DismissNotificationOfCurrentUserAsync(Guid id);
        Task<bool> DismissAllNotificationsOfCurrentUserAsync();
        Task<bool> UpdateCurrentUserPasswordAsync(string oldPassword, string newPassword);
    }
}