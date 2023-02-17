using HardwareShop.Business.Dtos;
using HardwareShop.Business.Extensions;
using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Business.Implementations
{
    public sealed class ShopService : IShopService
    {
        private readonly IRepository<Shop> shopRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Warehouse> warehouseRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly IRepository<UserShop> userShopRepository;
        private readonly IRepository<ShopAsset> shopAssetRepository;

        public ShopService(IRepository<Shop> shopRepository, IRepository<Warehouse> warehouseRepository, ICurrentUserService currentUserService, IResponseResultBuilder responseResultBuilder, IRepository<User> userRepository, IHashingPasswordService hashingPasswordService, IRepository<UserShop> userShopRepository, IRepository<ShopAsset> shopAssetRepository)
        {
            this.shopRepository = shopRepository;
            this.currentUserService = currentUserService;
            this.warehouseRepository = warehouseRepository;
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
            var user = await userRepository.CreateIfNotExistsAsync(new User
            {
                Username = username,
                HashedPassword = hashingPasswordService.Hash(password),
                Email = email,
            }, e => new
            {
                e.Username
            });
            if (user == null)
            {
                responseResultBuilder.AddExistedEntityError("User");
                return null;
            }

            UserShop userShop = await userShopRepository.CreateAsync(new UserShop
            {

                UserId = user.Id,
                ShopId = shop.Id,
                Role = UserShopRole.Admin
            });

            return new CreatedUserDto { Id = user.Id };

        }

        public async Task<CreatedShopDto?> CreateShopAsync(string name, string? address)
        {
            var shop = await shopRepository.CreateIfNotExistsAsync(new Shop
            {
                Name = name,
                Address = address
            },
                e => new
                {
                    e.Name,
                });
            if (shop == null)
            {
                responseResultBuilder.AddExistedEntityError("Shop");
                return null;
            }
            return new CreatedShopDto { Id = shop.Id };
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
            var userShop = await getUserShopByUserIdAsync(userId, role);
            if (userShop == null)
                return null;
            if (userShop.Shop == null)
                return null;
            return new ShopDto { Id = userShop.Shop.Id, UserRole = userShop.Role };
        }

        private async Task<UserShop?> getUserShopByUserIdAsync(int userId, UserShopRole role)
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
            var userShop = await getUserShopByUserIdAsync(currentUserService.GetUserId(), role);
            if (userShop == null)
            {
                return null;
            }
            return userShop.Shop;
        }
        public async Task<ShopDto?> GetShopDtoByCurrentUserIdAsync(UserShopRole role)
        {
            var userShop = await getUserShopByUserIdAsync(currentUserService.GetUserId(), role);
            if (userShop == null)
            {
                return null;
            }
            if (userShop.Shop == null)
                return null;
            return new ShopDto { Id = userShop.Shop.Id, UserRole = userShop.Role };
        }
        private async Task<ShopAssetDto> updateShopLogo(Shop shop, IFormFile file)
        {
            ShopAsset shopAsset = new ShopAsset() { AssetType = ShopAssetConstants.LogoAssetType, CreatedDate = DateTime.UtcNow, LastModifiedDate = DateTime.UtcNow, ShopId = shop.Id, };
            shopAsset = file.ConvertToAsset(shopAsset);
            shopAsset = await shopAssetRepository.CreateOrUpdateAsync(shopAsset, e => new { e.ShopId, e.AssetType }, e => new { e.Bytes, e.ContentType, e.Filename, e.LastModifiedDate });
            return new ShopAssetDto { Id = shopAsset.Id };
        }
        public async Task<ShopAssetDto?> UpdateLogoAsync(int shopId, IFormFile file)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == shopId);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            return await updateShopLogo(shop, file);
        }

        public async Task<ShopAssetDto?> UpdateYourShopLogoAsync(IFormFile file)
        {
            var shop = await GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            return await updateShopLogo(shop, file);
        }
    }
}
