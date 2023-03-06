using System.Text.Json;
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

        private async Task<IAssetTable?> getUserAvatarByUserId(int userId)
        {
            var user = await userRepository.GetItemByQueryAsync(e => e.Id == userId);
            if (user == null) return null;

            var userAsset = await userAssetRepository.GetItemByQueryAsync(e => e.UserId == user.Id
&& e.AssetType == UserAssetConstants.AvatarAssetType);
            return userAsset;
        }
        public Task<IAssetTable?> GetCurrentUserAvatarAsync()
        {
            var userId = currentUserService.GetUserId();
            return getUserAvatarByUserId(userId);
        }

        public async Task<List<UserDto>> GetUserDtosAsync()
        {
            PageData<User> userPageData = await userRepository.GetPageDataByQueryAsync(new PagingModel { PageIndex = 0, PageSize = 5 }, e => true, null, new List<QueryOrder<User>>() { new QueryOrder<User>(e => e.Username, true), new QueryOrder<User>(e => e.HashedPassword, false) });
            return userPageData.Items.Select(e => new UserDto() { Id = e.Id }).ToList();
        }

        public async Task<LoginDto?> LoginAsync(string username, string password)
        {
            var user = await userRepository.GetItemByQueryAsync(e => e.Username == username);
            if (user == null) return null;
            if (!hashingPasswordService.Verify(password, user.HashedPassword ?? ""))
            {
                return null;
            }
            return generateLoginDtoFromUser(user);
        }

        private LoginDto? generateLoginDtoFromUser(User user)
        {

            var cacheUser = new CacheUser()
            {
                Id = user.Id,
                Username = user.Username ?? "",
                Role = user.Role,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            var tokens = jwtService.GenerateTokens(cacheUser);
            if (tokens == null) return null;
            var userShop = user.UserShop;


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
            var user = await userRepository.GetItemByQueryAsync(e => e.Id == cacheUser.Id);
            if (user == null) return null;
            return generateLoginDtoFromUser(user);
        }

        public async Task<PageData<UserDto>?> GetUserPageDataOfShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            PageData<User> users = await userRepository.GetPageDataByQueryAsync(pagingModel, e => (e.UserShop != null && e.UserShop.ShopId == shop.Id), search == null ? null : new SearchQuery<User>(search, e => new
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
            var users = await userRepository.GetPageDataByQueryAsync(pagingModel, e => true, string.IsNullOrEmpty(search) ? null : new SearchQuery<User>(search, e => new
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
            var user = await getCurrentUserAsync();
            if (user == null) return false;

            user.InterfaceSettings = settings;
            await userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<PageData<NotificationDto>?> GetNotificationDtoPageDataOfCurrentUserAsync(PagingModel pagingModel)
        {
            var user = await getCurrentUserAsync();
            if (user == null) return null;
            return await notificationRepository.GetDtoPageDataByQueryAsync<NotificationDto>(pagingModel, e => e.UserId == user.Id && e.IsDismissed == false, e => new NotificationDto
            {
                Id = e.Id,
                CreatedDate = e.CreatedDate,
                Message = e.Message,
                Translation = e.Translation,
                TranslationParams = e.TranslationParams,
                Options = JsonDocument.Parse(JsonSerializer.Serialize(new
                {
                    Variant = e.Variant
                }, JsonSerializerConstants.CamelOptions))
            }, null, new List<QueryOrder<Notification>>() { new QueryOrder<Notification>(e => e.CreatedDate, false) });


        }
        private async Task<User?> getCurrentUserAsync()
        {
            var currentUserId = currentUserService.GetUserId();
            var user = await userRepository.GetItemByQueryAsync(e => e.Id == currentUserId);
            if (user == null)
            {
                responseResultBuilder.AddNotFoundEntityError("User");
                return null;
            }
            return user;
        }


        public async Task<bool> DismissNotificationOfCurrentUserAsync(Guid id)
        {
            var user = await getCurrentUserAsync();
            if (user == null) return false;

            var notification = await notificationRepository.GetItemByQueryAsync(e => e.Id == id && e.UserId == user.Id);
            if (notification == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Notification");
                return false;
            }
            notification.IsDismissed = true;
            await notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<bool> DismissAllNotificationsOfCurrentUserAsync()
        {
            var user = await getCurrentUserAsync();
            if (user == null) return false;
            var notifications = await notificationRepository.GetDataByQueryAsync(e => e.UserId == user.Id && e.IsDismissed == false);
            foreach (var notification in notifications)
            {
                notification.IsDismissed = true;
                await notificationRepository.UpdateAsync(notification);
            }
            return true;

        }

        public async Task<CreatedNotificationDto?> CreateNotificationOfCurrentUserAsync(string? message, string variant, string? translation, JsonDocument? translationParams)
        {
            var user = await getCurrentUserAsync();
            if (user == null) return null;
            var notification = await notificationRepository.CreateAsync(new Notification
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
            var user = await getCurrentUserAsync();
            if (user == null) return false;
            if (!hashingPasswordService.Verify(oldPassword, user.HashedPassword))
            {
                responseResultBuilder.AddInvalidFieldError("OldPassword");
                return false;
            }

            user.HashedPassword = hashingPasswordService.Hash(newPassword);
            await userRepository.UpdateAsync(user);
            return true;
        }
    }
}
