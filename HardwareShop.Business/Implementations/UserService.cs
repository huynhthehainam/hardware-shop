﻿using System.Text.Json;
using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Constants;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserAsset> userAssetRepository;
        private readonly IRepository<Notification> notificationRepository;
        private readonly IJwtService jwtService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILanguageService languageService;
        private readonly IShopService shopService;

        public UserService(IRepository<Notification> notificationRepository, IRepository<User> userRepository, IJwtService jwtService, ICurrentUserService currentUserService, IRepository<UserAsset> userAssetRepository, IResponseResultBuilder responseResultBuilder, ILanguageService languageService, IHashingPasswordService hashingPasswordService, IShopService shopService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
            this.currentUserService = currentUserService;
            this.userAssetRepository = userAssetRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.languageService = languageService;
            this.hashingPasswordService = hashingPasswordService;
            this.shopService = shopService;
            this.notificationRepository = notificationRepository;
        }

        public Task<CreatedUserDto> CreateUserAsync(string username, string password)
        {
            return Task.FromResult(new CreatedUserDto { Id = 1 });
        }

        private async Task<IAssetTable?> GetUserAvatarByUserId(int userId)
        {
            User? user = await userRepository.GetItemByQueryAsync(e => e.Id == userId);
            if (user == null)
            {
                return null;
            }

            UserAsset? userAsset = await userAssetRepository.GetItemByQueryAsync(e => e.UserId == user.Id
&& e.AssetType == UserAssetConstants.AvatarAssetType);
            return userAsset;
        }
        public Task<IAssetTable?> GetCurrentUserAvatarAsync()
        {
            int userId = currentUserService.GetUserId();
            return GetUserAvatarByUserId(userId);
        }

        public async Task<List<UserDto>> GetUserDtosAsync()
        {
            PageData<User> userPageData = await userRepository.GetPageDataByQueryAsync(new PagingModel { PageIndex = 0, PageSize = 5 }, e => true, null, new List<QueryOrder<User>>() { new QueryOrder<User>(e => e.Username, true), new QueryOrder<User>(e => e.HashedPassword, false) });
            return userPageData.Items.Select(e => new UserDto() { Id = e.Id }).ToList();
        }

        public async Task<LoginDto?> LoginAsync(string username, string password)
        {
            User? user = await userRepository.GetItemByQueryAsync(e => e.Username == username);
            return user == null
                ? null
                : !hashingPasswordService.Verify(password, user.HashedPassword ?? "") ? null : GenerateLoginDtoFromUser(user);
        }

        private LoginDto? GenerateLoginDtoFromUser(User user)
        {

            CacheUser cacheUser = new()
            {
                Id = user.Id,
                Username = user.Username ?? "",
                Role = user.Role,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            LoginResponse? tokens = jwtService.GenerateTokens(cacheUser);
            if (tokens == null)
            {
                return null;
            }

            UserShop? userShop = user.UserShop;


            return new LoginDto(tokens.AccessToken, new LoginUserDto(user.Role, new LoginUserDataDto(
                languageService.GenerateFullName(user.FirstName, user.LastName), user.Email, user.InterfaceSettings), (userShop == null || userShop.Shop == null) ? null : new LoginShopDto(userShop.Shop?.Id ?? 0,
                 userShop.Shop?.Name ?? "", userShop.Role, userShop.Shop?.CashUnit?.Name ?? "", userShop.Shop?.CashUnitId ?? 0, userShop.Shop?.Phones, userShop.Shop?.PhoneOwners, userShop.Shop?.Emails, userShop.Shop?.Address
                 )), tokens.SessionId);
        }
        public async Task<LoginDto?> LoginByTokenAsync(string token)
        {
            CacheUser? cacheUser = await jwtService.GetUserFromTokenAsync(token);
            if (cacheUser == null)
            {
                return null;
            }
            User? user = await userRepository.GetItemByQueryAsync(e => e.Id == cacheUser.Id);
            return user == null ? null : GenerateLoginDtoFromUser(user);
        }

        public async Task<PageData<UserDto>?> GetUserPageDataOfShopAsync(PagingModel pagingModel, string? search)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            PageData<User> users = await userRepository.GetPageDataByQueryAsync(pagingModel, e => e.UserShop != null && e.UserShop.ShopId == shop.Id, search == null ? null : new SearchQuery<User>(search, e => new
            {
                e.Email,
                e.Username,
                e.FirstName,
                e.LastName,
                e.Phone,
            }));
            return PageData<UserDto>.ConvertFromOtherPageData(users, e => new UserDto
            {
                Id = e.Id,
                Email = e.Email,
                FullName = languageService.GenerateFullName(e.FirstName, e.LastName),
                Phone = e.Phone,
                Username = e.Username,
            });
        }

        public async Task<PageData<UserDto>> GetUserPageDataAsync(PagingModel pagingModel, string? search)
        {
            PageData<User> users = await userRepository.GetPageDataByQueryAsync(pagingModel, e => true, string.IsNullOrEmpty(search) ? null : new SearchQuery<User>(search, e => new
            {
                e.Email,
                e.FirstName,
                e.LastName,
                e.Username,
                e.Phone
            }));
            return PageData<UserDto>.ConvertFromOtherPageData(users, e => new UserDto
            {
                Id = e.Id,
                Email = e.Email,
                FullName = languageService.GenerateFullName(e.FirstName, e.LastName),
                Phone = e.Phone,
                Username = e.Username,
            });
        }

        public async Task<bool> UpdateCurrentUserInterfaceSettings(JsonDocument settings)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            user.InterfaceSettings = settings;
            _ = await userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<PageData<NotificationDto>?> GetNotificationDtoPageDataOfCurrentUserAsync(PagingModel pagingModel)
        {
            User? user = await GetCurrentUserAsync();
            return user == null
                ? null
                : await notificationRepository.GetDtoPageDataByQueryAsync(pagingModel, e => e.UserId == user.Id && e.IsDismissed == false, e => new NotificationDto
                {
                    Id = e.Id,
                    CreatedDate = e.CreatedDate,
                    Message = e.Message,
                    Translation = e.Translation,
                    TranslationParams = e.TranslationParams,
                    Options = JsonDocument.Parse(JsonSerializer.Serialize(new
                    {
                        e.Variant
                    }, JsonSerializerConstants.CamelOptions))
                }, null, new List<QueryOrder<Notification>>() { new QueryOrder<Notification>(e => e.CreatedDate, false) });
        }
        private async Task<User?> GetCurrentUserAsync()
        {
            int currentUserId = currentUserService.GetUserId();
            User? user = await userRepository.GetItemByQueryAsync(e => e.Id == currentUserId);
            if (user == null)
            {
                responseResultBuilder.AddNotFoundEntityError("User");
                return null;
            }
            return user;
        }


        public async Task<bool> DismissNotificationOfCurrentUserAsync(Guid id)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            Notification? notification = await notificationRepository.GetItemByQueryAsync(e => e.Id == id && e.UserId == user.Id);
            if (notification == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Notification");
                return false;
            }
            notification.IsDismissed = true;
            _ = await notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<bool> DismissAllNotificationsOfCurrentUserAsync()
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            List<Notification> notifications = await notificationRepository.GetDataByQueryAsync(e => e.UserId == user.Id && e.IsDismissed == false);
            foreach (Notification notification in notifications)
            {
                notification.IsDismissed = true;
                _ = await notificationRepository.UpdateAsync(notification);
            }
            return true;

        }

        public async Task<CreatedNotificationDto?> CreateNotificationOfCurrentUserAsync(string? message, string variant, string? translation, JsonDocument? translationParams)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return null;
            }

            Notification notification = await notificationRepository.CreateAsync(new Notification
            {
                CreatedDate = DateTime.UtcNow,
                Message = message,
                Variant = variant,
                UserId = user.Id,
                Translation = translation,
                TranslationParams = translationParams,
            });
            return new CreatedNotificationDto { Id = notification.Id };
        }

        public async Task<bool> UpdateCurrentUserPasswordAsync(string oldPassword, string newPassword)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            if (!hashingPasswordService.Verify(oldPassword, user.HashedPassword))
            {
                responseResultBuilder.AddInvalidFieldError("OldPassword");
                return false;
            }

            user.HashedPassword = hashingPasswordService.Hash(newPassword);
            _ = await userRepository.UpdateAsync(user);
            return true;
        }
    }
}
