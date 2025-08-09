using System.Text.Json;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Constants;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using HardwareShop.Application.Models;
using HardwareShop.Infrastructure.Extensions;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Infrastructure.Services
{
    public class UserService : IUserService
    {

        private readonly IJwtService jwtService;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly ICurrentUserService currentUserService;
        private readonly IShopService shopService;
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        public UserService(DbContext dbContext, IJwtService jwtService, ICurrentUserService currentUserService, IHashingPasswordService hashingPasswordService, IShopService shopService, IDistributedCache distributedCache)
        {
            this.jwtService = jwtService;
            this.currentUserService = currentUserService;
            this.hashingPasswordService = hashingPasswordService;
            this.shopService = shopService;
            this.db = dbContext;
            this.distributedCache = distributedCache;
        }

        public Task<CreatedUserDto> CreateUserAsync(string username, string password)
        {
            return Task.FromResult(new CreatedUserDto { Id = 1 });
        }

        private async Task<AssetEntityBase?> GetUserAvatarByUserId(Guid userId)
        {
            User? user = await db.Set<User>().FirstOrDefaultAsync(e => e.Guid == userId);
            if (user == null)
            {
                return null;
            }

            UserAsset? userAsset = await db.Set<UserAsset>().FirstOrDefaultAsync(e => e.UserId == user.Id
&& e.AssetType == UserAssetConstants.AvatarAssetType);
            return userAsset;
        }
        public async Task<ApplicationResponse<CachedAssetDto>> GetCurrentUserAvatarAsync()
        {
            Guid userId = currentUserService.GetUserGuid();
            var avatar = await GetUserAvatarByUserId(userId);
            if (avatar == null)
            {
                return new(ApplicationError.CreateNotFoundError("Avatar"));
            }
            var asset = db.GetCachedAssetById(distributedCache, avatar.AssetId);
            if (asset == null) return new(ApplicationError.CreateNotFoundError("Avatar"));
            return new(asset);
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

        private LoginDto GenerateLoginDtoFromUser(User user)
        {

            ApplicationUserDto cacheUser = new()
            {
                Username = user.Username ?? "",
                Role = user.Role,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Guid = user.Guid,
            };

            TokenDto tokens = jwtService.GenerateTokens(cacheUser);
            UserShop? userShop = user.UserShop;
            return new LoginDto(tokens.AccessToken, new LoginUserDto(user.Role, new LoginUserDataDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Guid = user.Guid,
                Settings = JsonDocument.Parse(user.InterfaceSettings),
            }, (userShop == null || userShop.Shop == null) ? null : new LoginShopDto(userShop.Shop?.Id ?? 0,
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
            ApplicationUserDto? cacheUser = jwtService.GetUserFromToken(token);
            if (cacheUser == null)
            {
                return null;
            }
            User? user = await db.Set<User>().FirstOrDefaultAsync(e => e.Guid == cacheUser.Guid);
            return user == null ? null : GenerateLoginDtoFromUser(user);
        }

        public async Task<ApplicationResponse<PageData<UserDto>>> GetUserPageDataOfShopAsync(PagingModel pagingModel, string? search)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            var userPageData = db.Set<User>().Where(e => e.UserShop != null && e.UserShop.ShopId == shop.Id).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<User>(search, e => new
            {
                e.Email,
                e.Username,
                e.FirstName,
                e.LastName,
                e.Phone,
            })).GetPageData(pagingModel);
            return new(userPageData.ConvertToOtherPageData(e => new UserDto
            {
                Id = e.Id,
                Email = e.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Phone = e.Phone,
                Username = e.Username,
            }));
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
                FirstName = e.FirstName,
                LastName = e.LastName,
                Phone = e.Phone,
                Username = e.Username,
            });
        }

        public async Task<ApplicationResponse> UpdateCurrentUserInterfaceSettings(JsonDocument settings)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return new(ApplicationError.CreateNotFoundError("User"));
            }

            user.InterfaceSettings = JsonSerializer.Serialize(settings.RootElement, JsonSerializerConstants.CamelOptions); ;
            db.Update(user);
            db.SaveChanges();
            return new();
        }

        public async Task<ApplicationResponse<PageData<NotificationDto>>> GetNotificationDtoPageDataOfCurrentUserAsync(PagingModel pagingModel)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return new(ApplicationError.CreateNotFoundError("User"));
            }
            var notificationPageData = await db.Set<Notification>().Where(e => e.UserId == user.Id && e.IsDismissed == false).GetPageDataAsync(pagingModel, new OrderQuery<Notification>[] { new OrderQuery<Notification>(e => e.CreatedDate, false) });
            return new(notificationPageData.ConvertToOtherPageData(e => new NotificationDto
            {
                Id = e.Id,
                CreatedDate = e.CreatedDate,
                Message = e.Message,
                Translation = e.Translation,
                TranslationParams = string.IsNullOrEmpty(e.TranslationParams) ? null : JsonDocument.Parse(e.TranslationParams),
                Options = JsonDocument.Parse(JsonSerializer.Serialize(new
                {
                    e.Variant
                }, JsonSerializerConstants.CamelOptions))
            }));

        }
        private async Task<User?> GetCurrentUserAsync()
        {
            Guid currentUserId = currentUserService.GetUserGuid();
            User? user = await db.Set<User>().FirstOrDefaultAsync(e => e.Guid == currentUserId);

            return user;
        }


        public async Task<ApplicationResponse> DismissNotificationOfCurrentUserAsync(Guid id)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return new(ApplicationError.CreateNotFoundError("User"));
            }

            Notification? notification = await db.Set<Notification>().FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);
            if (notification == null)
            {
                return new(ApplicationError.CreateNotFoundError("Notification"));

            }
            notification.IsDismissed = true;
            db.Update(notification);
            db.SaveChanges();
            return new();
        }

        public async Task<ApplicationResponse> DismissAllNotificationsOfCurrentUserAsync()
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return new(ApplicationError.CreateNotFoundError("User"));
            }

            var notifications = db.Set<Notification>().Where(e => e.UserId == user.Id && e.IsDismissed == false).ToArray();
            foreach (Notification notification in notifications)
            {
                notification.IsDismissed = true;
                db.Update(notification);
            }
            db.SaveChanges();
            return new();

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
                TranslationParams = translationParams is null ? null : JsonSerializer.Serialize(translationParams.RootElement, JsonSerializerConstants.CamelOptions),
            };
            db.Add(notification);
            db.SaveChanges();
            return new CreatedNotificationDto { Id = notification.Id };
        }

        public async Task<ApplicationResponse> UpdateCurrentUserPasswordAsync(string oldPassword, string newPassword)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
            {
                return new(ApplicationError.CreateNotFoundError("User"));
            }

            if (!hashingPasswordService.Verify(oldPassword, user.HashedPassword))
            {
                return new(ApplicationError.CreateInvalidError("OldPassword"));

            }

            user.HashedPassword = hashingPasswordService.Hash(newPassword);
            db.Update(user); db.SaveChanges();
            return new();
        }


    }
}
