using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Infrastructure.Services
{
    public sealed class ShopService : IShopService
    {

        private readonly ICurrentUserService currentUserService;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        private readonly IUnitService unitService;

        public ShopService(DbContext db, ICurrentUserService currentUserService, IHashingPasswordService hashingPasswordService, IDistributedCache distributedCache, IUnitService unitService)
        {
            this.distributedCache = distributedCache;
            this.db = db;
            this.currentUserService = currentUserService;
            this.unitService = unitService;
            this.hashingPasswordService = hashingPasswordService;
        }

        public async Task<ApplicationResponse<CreatedUserDto>> CreateAdminUserAsync(int id, string username, string password, string? email)
        {
            if (currentUserService.IsSystemAdmin())
            {
                return new(ApplicationError.CreateNotPermittedError());
            }
            return new(ApplicationError.CreateNotPermittedError());

        }

        public ApplicationResponse<CreatedShopDto> CreateShop(string name, string? address, int cashUnitId)
        {
            var isCashUnitExist = unitService.IsCashUnitExist(cashUnitId);
            if (!isCashUnitExist)
            {
                return new(ApplicationError.CreateInvalidError("CashUnit"));

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
                return new(ApplicationError.CreateExistedError("Shop"));

            }
            return new(new CreatedShopDto { Id = createIfNotExistResponse.Entity.Id });
        }



        public async Task<ApplicationResponse> DeleteShopSoftlyAsync(int shopId)
        {
            var shop = await db.Set<Shop>().FirstOrDefaultAsync(e => e.Id == shopId);
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            db.SoftDelete(shop);
            return new();
        }

        public async Task<ShopDto?> GetShopByUserIdAsync(Guid userId)
        {
            var userShop = await GetUserShopByUserIdAsync(userId);
            if (userShop == null)
                return null;
            if (userShop.Shop == null)
                return null;
            return new ShopDto { Id = userShop.Shop.Id, UserRole = userShop.Role.ToString() };
        }

        private async Task<UserShop?> GetUserShopByUserIdAsync(Guid userGuid)
        {
            var acceptedRoles = new List<UserShopRole> { UserShopRole.Staff, UserShopRole.Admin };


            var userShop = await db.Set<UserShop>().Include(e => e.Shop).FirstOrDefaultAsync(e => e.User!.Id == userGuid && acceptedRoles.Contains(e.Role));

            return userShop;
        }
        public async Task<Shop?> GetShopByCurrentUserIdAsync()
        {
            var userShop = await GetUserShopByUserIdAsync(currentUserService.GetUserGuid());
            if (userShop == null)
            {
                return null;
            }
            return userShop.Shop;
        }
        public async Task<ShopDto?> GetShopDtoByCurrentUserIdAsync()
        {
            var userShop = await GetUserShopByUserIdAsync(currentUserService.GetUserGuid());
            if (userShop == null)
            {
                return null;
            }
            if (userShop.Shop == null)
                return null;
            return new ShopDto { Id = userShop.Shop.Id, UserRole = userShop.Role.ToString() };
        }
        private Task<ShopAssetDto> UpdateShopLogo(Shop shop, AssetDto file)
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
        public async Task<ApplicationResponse<ShopAssetDto>> UpdateLogoAsync(int shopId, AssetDto file)
        {
            var shop = await db.Set<Shop>().FirstOrDefaultAsync(e => e.Id == shopId);
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            return new(await UpdateShopLogo(shop, file));
        }

        public async Task<ApplicationResponse<ShopAssetDto>> UpdateYourShopLogoAsync(AssetDto file)
        {
            var shop = await GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            return new(await UpdateShopLogo(shop, file));
        }

        public async Task<ApplicationResponse<CachedAssetDto>> GetCurrentUserShopLogoAsync()
        {
            var shop = await GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            var logo = await db.Set<ShopAsset>().FirstOrDefaultAsync(e => e.ShopId == shop.Id && e.AssetType == ShopAssetConstants.LogoAssetType);
            if (logo == null)
            {
                return new(ApplicationError.CreateNotFoundError("Logo"));
            }
            var asset = db.GetCachedAssetById(distributedCache, logo.AssetId);
            if (asset == null) return new(ApplicationError.CreateNotFoundError("Asset"));
            return new(asset);
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

        public async Task<ApplicationResponse> UpdateShopSettingAsync(int shopId, bool? isAllowedToShowInvoiceDownloadOptions)
        {
            var shopSetting = await db.Set<ShopSetting>().FirstOrDefaultAsync(e => e.ShopId == shopId && e.Shop != null && e.Shop.UserShops != null && e.Shop.UserShops.Any(e => e.User!.Id == currentUserService.GetUserGuid() && e.Role == UserShopRole.Admin));
            if (shopSetting == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            if (isAllowedToShowInvoiceDownloadOptions.HasValue)
            {
                shopSetting.IsAllowedToShowInvoiceDownloadOptions = isAllowedToShowInvoiceDownloadOptions.HasValue;
            }
            db.Entry(shopSetting).State = EntityState.Modified;
            db.SaveChanges();
            return new();
        }


    }


}
