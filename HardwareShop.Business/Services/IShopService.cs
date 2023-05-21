using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Business.Services
{
    public interface IShopService
    {
        Task<ShopDto?> GetShopByUserIdAsync(int userId, UserShopRole role = UserShopRole.Staff);
        Task<ShopDto?> GetShopDtoByCurrentUserIdAsync(UserShopRole role = UserShopRole.Staff);
        Task<CreatedShopDto?> CreateShopAsync(string name, string? address);
        Task<ShopAssetDto?> UpdateLogoAsync(int shopId, IFormFile file);
        Task<bool> DeleteShopSoftlyAsync(int shopId);
        Task<bool> UpdateShopSettingAsync(int shopId, bool? isAllowedToShowInvoiceDownloadOptions);
        Task<CreatedUserDto?> CreateAdminUserAsync(int id, string v1, string v2, string? email);
        Task<Shop?> GetShopByCurrentUserIdAsync(UserShopRole role = UserShopRole.Staff);
        Task<ShopAssetDto?> UpdateYourShopLogoAsync(IFormFile logo);
        Task<CachedAsset?> GetCurrentUserShopLogo();
        Task<PageData<ShopItemDto>> GetShopDtoPageDataAsync(PagingModel pagingModel, string? search);
    }
}
