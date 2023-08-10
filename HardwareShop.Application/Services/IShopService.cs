using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IShopService
    {
        Task<ShopDto?> GetShopByUserIdAsync(Guid userId);
        Task<ShopDto?> GetShopDtoByCurrentUserIdAsync();
        Task<ApplicationResponse<CreatedShopDto>> CreateShopAsync(string name, string? address, int cashUnitId);
        Task<ApplicationResponse<ShopAssetDto>> UpdateLogoAsync(int shopId, AssetDto file);
        Task<ApplicationResponse> DeleteShopSoftlyAsync(int shopId);
        Task<ApplicationResponse> UpdateShopSettingAsync(int shopId, bool? isAllowedToShowInvoiceDownloadOptions);
        Task<ApplicationResponse<CreatedUserDto>> CreateAdminUserAsync(int id, string v1, string v2, string? email);
        Task<Shop?> GetShopByCurrentUserIdAsync();
        Task<ApplicationResponse<ShopAssetDto>> UpdateYourShopLogoAsync(AssetDto logo);
        Task<ApplicationResponse<CachedAssetDto>> GetCurrentUserShopLogoAsync();
        Task<PageData<ShopItemDto>> GetShopDtoPageDataAsync(PagingModel pagingModel, string? search);
    }
}
