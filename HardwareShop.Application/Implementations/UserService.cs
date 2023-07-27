using System.Text.Json;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Constants;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Application.Implementations
{
    public class UserService : IUserService
    {

        private readonly IJwtService jwtService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILanguageService languageService;
        private readonly IShopService shopService;
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        public UserService(DbContext dbContext, IJwtService jwtService, ICurrentUserService currentUserService, IResponseResultBuilder responseResultBuilder, ILanguageService languageService, IHashingPasswordService hashingPasswordService, IShopService shopService, IDistributedCache distributedCache)
        {
            this.jwtService = jwtService;
            this.currentUserService = currentUserService;
            this.responseResultBuilder = responseResultBuilder;
            this.languageService = languageService;
            this.hashingPasswordService = hashingPasswordService;
            this.shopService = shopService;
            this.db = dbContext;
            this.distributedCache = distributedCache;
        }

        public Task<CreatedUserDto> CreateUserAsync(string username, string password)
        {
            return Task.FromResult(new CreatedUserDto { Id = 1 });
        }

        private async Task<AssetEntityBase?> GetUserAvatarByUserId(int userId)
        {
            User? user = await db.Set<User>().FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return null;
            }

            UserAsset? userAsset = await db.Set<UserAsset>().FirstOrDefaultAsync(e => e.UserId == user.Id
&& e.AssetType == UserAssetConstants.AvatarAssetType);
            return userAsset;
        }
        public async Task<CachedAsset?> GetCurrentUserAvatarAsync()
        {
            int userId = currentUserService.GetUserId();
            var avatar = await GetUserAvatarByUserId(userId);
            if (avatar == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Avatar");
                return null;
            }
            return db.GetCachedAssetById(distributedCache, avatar.AssetId);
        }



        public async Task<LoginDto?> LoginAsync(string username, string password)
        {
            User? user = await db.Set<User>().FirstOrDefaultAsync(e => e.Username == username);
            if (user != null)
            {
                var aa = hashingPasswordService.Verify(password, user.HashedPassword ?? "");
            }
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
                Guid = user.Guid,
            };

            LoginResponse? tokens = jwtService.GenerateTokens(cacheUser);
            if (tokens == null)
            {
                return null;
            }

            UserShop? userShop = user.UserShop;


            return new LoginDto(tokens.AccessToken, new LoginUserDto(user.Role, new LoginUserDataDto(
                languageService.GenerateFullName(user.FirstName, user.LastName), user.Email, user.InterfaceSettings, user.Guid), (userShop == null || userShop.Shop == null) ? null : new LoginShopDto(userShop.Shop?.Id ?? 0,
                 userShop.Shop?.Name ?? "", userShop.Role, userShop.Shop?.CashUnit?.Name ?? "", userShop.Shop?.CashUnitId ?? 0, userShop.Shop?.Phones?.Select(e => new ShopPhoneDto()
                 {
                     Id = e.Id,
                     OwnerName = e.OwnerName,
                     Phone = e.Phone,
                     PhonePrefix = e.Country?.PhonePrefix ?? ""
                 }) ?? Array.Empty<ShopPhoneDto>(), userShop.Shop?.Emails, userShop.Shop?.Address
                , userShop.Shop?.ShopSetting?.IsAllowedToShowInvoiceDownloadOptions ?? false)), tokens.SessionId);
        }
        public async Task<LoginDto?> LoginByTokenAsync(string token)
        {
            CacheUser? cacheUser = await jwtService.GetUserFromTokenAsync(token);
            if (cacheUser == null)
            {
                return null;
            }
            User? user = await db.Set<User>().FirstOrDefaultAsync(e => e.Id == cacheUser.Id);
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
            var userPageData = db.Set<User>().Where(e => e.UserShop != null && e.UserShop.ShopId == shop.Id).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<User>(search, e => new
            {
                e.Email,
                e.Username,
                e.FirstName,
                e.LastName,
                e.Phone,
            })).GetPageData(pagingModel);
            return userPageData.ConvertToOtherPageData(e => new UserDto
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
            var userPageData = await db.Set<User>().Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<User>(search, e => new
            {
                e.Email,
                e.FirstName,
                e.LastName,
                e.Username,
                e.Phone
            })).GetPageDataAsync(pagingModel);
            return userPageData.ConvertToOtherPageData(e => new UserDto
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
                responseResultBuilder.AddNotFoundEntityError("User");
                return false;
            }

            user.InterfaceSettings = settings;
            db.Update(user);
            db.SaveChanges();
            return true;
        }

        public async Task<PageData<NotificationDto>?> GetNotificationDtoPageDataOfCurrentUserAsync(PagingModel pagingModel)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                responseResultBuilder.AddNotFoundEntityError("User");
                return null;
            }
            var notificationPageData = await db.Set<Notification>().Where(e => e.UserId == user.Id && e.IsDismissed == false).GetPageDataAsync(pagingModel, new OrderQuery<Notification>[] { new OrderQuery<Notification>(e => e.CreatedDate, false) });
            return notificationPageData.ConvertToOtherPageData(e => new NotificationDto
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
            });

        }
        private async Task<User?> GetCurrentUserAsync()
        {
            int currentUserId = currentUserService.GetUserId();
            User? user = await db.Set<User>().FirstOrDefaultAsync(e => e.Id == currentUserId);
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

            Notification? notification = await db.Set<Notification>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);
            if (notification == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Notification");
                return false;
            }
            notification.IsDismissed = true;
            db.Update(notification);
            db.SaveChanges();
            return true;
        }

        public async Task<bool> DismissAllNotificationsOfCurrentUserAsync()
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            var notifications = db.Set<Notification>().Where(e => e.UserId == user.Id && e.IsDismissed == false).ToArray();
            foreach (Notification notification in notifications)
            {
                notification.IsDismissed = true;
                db.Update(notification);
            }
            db.SaveChanges();
            return true;

        }

        public async Task<CreatedNotificationDto?> CreateNotificationOfCurrentUserAsync(string? message, string variant, string? translation, JsonDocument? translationParams)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return null;
            }

            Notification notification = new Notification
            {
                CreatedDate = DateTime.UtcNow,
                Message = message,
                Variant = variant,
                UserId = user.Id,
                Translation = translation,
                TranslationParams = translationParams,
            };
            db.Add(notification);
            db.SaveChanges();
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
            db.Update(user); db.SaveChanges();
            return true;
        }


    }
}
