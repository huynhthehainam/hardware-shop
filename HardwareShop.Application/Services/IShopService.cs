using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IShopService
    {
        Task<ShopDto?> GetShopByUserIdAsync(Guid userId);
        Task<ShopDto?> GetShopDtoByCurrentUserIdAsync();
        ApplicationResponse<CreatedShopDto> CreateShop(string name, string? address, int cashUnitId);
        Task<ApplicationResponse<ShopAssetDto>> UpdateLogoAsync(Guid shopId, AssetDto file);
        Task<ApplicationResponse> DeleteShopSoftlyAsync(Guid shopId);
        Task<ApplicationResponse> UpdateShopSettingAsync(Guid shopId, bool? isAllowedToShowInvoiceDownloadOptions);
        Task<ApplicationResponse<CreatedUserDto>> CreateAdminUserAsync(int id, string v1, string v2, string? email);
        Task<Shop?> GetShopByCurrentUserIdAsync();
        Task<ApplicationResponse<ShopAssetDto>> UpdateYourShopLogoAsync(AssetDto logo);
        Task<ApplicationResponse<CachedAssetDto>> GetCurrentUserShopLogoAsync();
        Task<PageData<ShopItemDto>> GetShopDtoPageDataAsync(PagingModel pagingModel, string? search);
    }
}
