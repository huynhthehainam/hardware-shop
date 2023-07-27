using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Application.Implementations
{
    public sealed class ShopService : IShopService
    {

        private readonly ICurrentUserService currentUserService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        private readonly IUnitService unitService;

        public ShopService(DbContext db, ICurrentUserService currentUserService, IResponseResultBuilder responseResultBuilder, IHashingPasswordService hashingPasswordService, IDistributedCache distributedCache, IUnitService unitService)
        {
            this.distributedCache = distributedCache;
            this.db = db;
            this.currentUserService = currentUserService;
            this.responseResultBuilder = responseResultBuilder;
            this.unitService = unitService;
            this.hashingPasswordService = hashingPasswordService;
        }

        public async Task<CreatedUserDto?> CreateAdminUserAsync(int id, string username, string password, string? email)
        {
            var shop = await db.Set<Shop>().FirstOrDefaultAsync(e => e.Id == id);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var createIfNotExistResponse = db.CreateIfNotExists(new User
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

            UserShop userShop = new UserShop
            {

                UserId = createIfNotExistResponse.Entity.Id,
                ShopId = shop.Id,
                Role = UserShopRole.Admin
            };
            db.Add(userShop);
            db.SaveChanges();

            return new CreatedUserDto { Id = createIfNotExistResponse.Entity.Id };

        }

        public Task<CreatedShopDto?> CreateShopAsync(string name, string? address, int cashUnitId)
        {
            var isCashUnitExist = unitService.IsCashUnitExist(cashUnitId);
            if (!isCashUnitExist)
            {
                responseResultBuilder.AddInvalidFieldError("CashUnit");
                return Task.FromResult<CreatedShopDto?>(null);
            }
            var createIfNotExistResponse = db.CreateIfNotExists(new Shop
            {
                Name = name,
                Address = address,
                CashUnitId = cashUnitId,
            },
                e => new
                {
                    e.Name,
                });
            if (createIfNotExistResponse.IsExist)
            {
                responseResultBuilder.AddExistedEntityError("Shop");
                return Task.FromResult<CreatedShopDto?>(null);
            }
            return Task.FromResult<CreatedShopDto?>(new CreatedShopDto { Id = createIfNotExistResponse.Entity.Id });
        }



        public async Task<bool> DeleteShopSoftlyAsync(int shopId)
        {
            var shop = await db.Set<Shop>().FirstOrDefaultAsync(e => e.Id == shopId);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            return db.SoftDelete(shop);
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
            var userShop = await db.Set<UserShop>().Include(e => e.Shop).FirstOrDefaultAsync(e => e.UserId == userId && acceptedRoles.Contains(e.Role));

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
        private Task<ShopAssetDto> UpdateShopLogo(Shop shop, IFormFile file)
        {
            ShopAsset shopAsset = new()
            {
                AssetType = ShopAssetConstants.LogoAssetType,
                ShopId = shop.Id,
            };
            shopAsset = file.ConvertToAsset(shopAsset);
            var createOrUpdateResponse = db.CreateOrUpdateAsset(shopAsset, e => new { e.ShopId, e.AssetType }, e => new
            {
                e.ShopId,
                e.AssetType,
            });

            return Task.FromResult(new ShopAssetDto { Id = createOrUpdateResponse.Entity.Id });
        }
        public async Task<ShopAssetDto?> UpdateLogoAsync(int shopId, IFormFile file)
        {
            var shop = await db.Set<Shop>().FirstOrDefaultAsync(e => e.Id == shopId);
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

        public async Task<CachedAsset?> GetCurrentUserShopLogoAsync()
        {
            var shop = await GetShopByCurrentUserIdAsync(UserShopRole.Staff);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var logo = await db.Set<ShopAsset>().FirstOrDefaultAsync(e => e.ShopId == shop.Id && e.AssetType == ShopAssetConstants.LogoAssetType);
            if (logo == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Logo");
                return null;
            }
            return db.GetCachedAssetById(distributedCache, logo.AssetId);
        }

        public async Task<PageData<ShopItemDto>> GetShopDtoPageDataAsync(PagingModel pagingModel, string? search)
        {
            var shopDto = await db.Set<Shop>().Where(e => true).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Shop>(search, e => new
            {
                e.Address,
                e.Name
            })).GetPageDataAsync(pagingModel);
            return shopDto.ConvertToOtherPageData(e => new ShopItemDto()
            {
                Id = e.Id,
                Address = e.Address,
                Emails = e.Emails,
                Name = e.Name,
                CashUnitId = e.CashUnitId,
                Phones = e.Phones?.Select(sp => new ShopPhoneDto()
                {
                    Id = sp.Id,
                    OwnerName = sp.OwnerName,
                    Phone = sp.Phone,
                    PhonePrefix = sp.Country?.PhonePrefix ?? ""
                }).ToArray() ?? Array.Empty<ShopPhoneDto>()
            });
        }

        public async Task<bool> UpdateShopSettingAsync(int shopId, bool? isAllowedToShowInvoiceDownloadOptions)
        {
            var shopSetting = await db.Set<ShopSetting>().FirstOrDefaultAsync(e => e.ShopId == shopId && e.Shop != null && e.Shop.UserShops != null && e.Shop.UserShops.Any(e => e.UserId == currentUserService.GetUserId() && e.Role == UserShopRole.Admin));
            if (shopSetting == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            if (isAllowedToShowInvoiceDownloadOptions.HasValue)
            {
                shopSetting.IsAllowedToShowInvoiceDownloadOptions = isAllowedToShowInvoiceDownloadOptions.HasValue;
            }
            db.Entry(shopSetting).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }
    }
}
