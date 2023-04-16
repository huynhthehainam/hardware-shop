using HardwareShop.Business.Dtos;
using HardwareShop.Business.Extensions;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Business.Implementations
{
    public sealed class ShopService : IShopService
    {
        private readonly IRepository<Shop> shopRepository;
        private readonly IRepository<User> userRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly IRepository<UserShop> userShopRepository;
        private readonly IRepository<ShopAsset> shopAssetRepository;

        public ShopService(IRepository<Shop> shopRepository, ICurrentUserService currentUserService, IResponseResultBuilder responseResultBuilder, IRepository<User> userRepository, IHashingPasswordService hashingPasswordService, IRepository<UserShop> userShopRepository, IRepository<ShopAsset> shopAssetRepository)
        {
            this.shopRepository = shopRepository;
            this.currentUserService = currentUserService;
            this.responseResultBuilder = responseResultBuilder;
            this.userRepository = userRepository;
            this.hashingPasswordService = hashingPasswordService;
            this.userShopRepository = userShopRepository;
            this.shopAssetRepository = shopAssetRepository;
        }

        public async Task<CreatedUserDto?> CreateAdminUserAsync(int id, string username, string password, string? email)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == id);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var createIfNotExistResponse = await userRepository.CreateIfNotExistsAsync(new User
            {
                Username = username,
                HashedPassword = hashingPasswordService.Hash(password),
                Email = email,
            }, e => new
            {
                e.Username
            });
            if (createIfNotExistResponse.IsExist)
            {
                responseResultBuilder.AddExistedEntityError("User");
                return null;
            }

            UserShop userShop = await userShopRepository.CreateAsync(new UserShop
            {

                UserId = createIfNotExistResponse.Entity.Id,
                ShopId = shop.Id,
                Role = UserShopRole.Admin
            });

            return new CreatedUserDto { Id = createIfNotExistResponse.Entity.Id };

        }

        public async Task<CreatedShopDto?> CreateShopAsync(string name, string? address)
        {
            var createIfNotExistResponse = await shopRepository.CreateIfNotExistsAsync(new Shop
            {
                Name = name,
                Address = address
            },
                e => new
                {
                    e.Name,
                });
            if (createIfNotExistResponse.IsExist)
            {
                responseResultBuilder.AddExistedEntityError("Shop");
                return null;
            }
            return new CreatedShopDto { Id = createIfNotExistResponse.Entity.Id };
        }



        public async Task<bool> DeleteShopSoftlyAsync(int shopId)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == shopId);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            return await shopRepository.DeleteSoftlyAsync(shop);
        }

        public async Task<ShopDto?> GetShopByUserIdAsync(int userId, UserShopRole role = UserShopRole.Staff)
        {
            var userShop = await GetUserShopByUserIdAsync(userId, role);
            if (userShop == null)
                return null;
            if (userShop.Shop == null)
                return null;
            return new ShopDto { Id = userShop.Shop.Id, UserRole = userShop.Role };
        }

        private async Task<UserShop?> GetUserShopByUserIdAsync(int userId, UserShopRole role)
        {
            var acceptedRoles = new List<UserShopRole>();
            switch (role)
            {
                case UserShopRole.Staff:
                    acceptedRoles = new List<UserShopRole> { UserShopRole.Staff, UserShopRole.Admin };
                    break;
                case UserShopRole.Admin:
                    acceptedRoles = new List<UserShopRole> { UserShopRole.Admin };
                    break;
                default:
                    break;
            }
            var userShop = await userShopRepository.GetItemByQueryAsync(e => e.UserId == userId && acceptedRoles.Contains(e.Role));

            return userShop;
        }
        public async Task<Shop?> GetShopByCurrentUserIdAsync(UserShopRole role)
        {
            var userShop = await GetUserShopByUserIdAsync(currentUserService.GetUserId(), role);
            if (userShop == null)
            {
                return null;
            }
            return userShop.Shop;
        }
        public async Task<ShopDto?> GetShopDtoByCurrentUserIdAsync(UserShopRole role)
        {
            var userShop = await GetUserShopByUserIdAsync(currentUserService.GetUserId(), role);
            if (userShop == null)
            {
                return null;
            }
            if (userShop.Shop == null)
                return null;
            return new ShopDto { Id = userShop.Shop.Id, UserRole = userShop.Role };
        }
        private async Task<ShopAssetDto> UpdateShopLogo(Shop shop, IFormFile file)
        {
            ShopAsset shopAsset = new()
            {
                AssetType = ShopAssetConstants.LogoAssetType,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                ShopId = shop.Id,
            };
            shopAsset = file.ConvertToAsset(shopAsset);
            var createOrUpdateResponse = await shopAssetRepository.CreateOrUpdateAsync(shopAsset, e => new { e.ShopId, e.AssetType }, e => new { e.Bytes, e.ContentType, e.Filename, e.LastModifiedDate });

            return new ShopAssetDto { Id = createOrUpdateResponse.Entity.Id };
        }
        public async Task<ShopAssetDto?> UpdateLogoAsync(int shopId, IFormFile file)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == shopId);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            return await UpdateShopLogo(shop, file);
        }

        public async Task<ShopAssetDto?> UpdateYourShopLogoAsync(IFormFile file)
        {
            var shop = await GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            return await UpdateShopLogo(shop, file);
        }

        public async Task<IAssetTable?> GetCurrentUserShopLogo()
        {
            var shop = await GetShopByCurrentUserIdAsync(UserShopRole.Staff);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var logo = await shopAssetRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.AssetType == ShopAssetConstants.LogoAssetType);
            if (logo == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Logo");
                return null;
            }
            return logo;
        }

        public async Task<PageData<ShopItemDto>> GetShopDtoPageDataAsync(PagingModel pagingModel, string? search)
        {
            var shopDtoPageData = await shopRepository.GetDtoPageDataByQueryAsync<ShopItemDto>(pagingModel, e => true, e => new ShopItemDto()
            {
                Id = e.Id,
                Address = e.Address,
                Emails = e.Emails,
                Name = e.Name,
                Phones = e.Phones?.Select(sp => new ShopPhoneDto()
                {
                    Id = sp.Id,
                    OwnerName = sp.OwnerName,
                    Phone = sp.Phone,
                    PhonePrefix = sp.Country?.PhonePrefix ?? ""
                }).ToArray() ?? Array.Empty<ShopPhoneDto>()
            }, string.IsNullOrEmpty(search) ? null : new SearchQuery<Shop>(search, e => new
            {
                e.Address,
                e.Name
            }));
            return shopDtoPageData;
        }
    }
}
