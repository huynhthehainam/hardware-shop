using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;

namespace HardwareShop.Application.Services
{
    public interface IProductService
    {
        Task<ApplicationResponse<PageData<ProductDto>>> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search, SortingModel sortingModel);
        Task<ApplicationResponse<CreatedProductDto>> CreateProductOfShopAsync(string name, int unitId,
         double? mass,
         double? pricePerMass,
         double? percentForFamiliarCustomer,
         double? percentForCustomer,
         double? priceForFamiliarCustomer,
         double priceForCustomer,
         bool hasAutoCalculatePermission,
         List<int>? categoryIds,
         List<Tuple<int, double>>? warehouses
         );
        Task<ApplicationResponse<CachedAssetDto>> GetProductThumbnailAsync(int productId);
        Task<ApplicationResponse<CachedAssetDto>> GetProductAssetByIdAsync(int productId, int assetId);
        Task<ApplicationResponse<int>> UploadProductImageOfCurrentUserShopAsync(int productId, string assetType, AssetDto file);
        Task<ApplicationResponse<ProductDto>> GetProductDtoOfCurrentUserShopAsync(int productId);
        Task<ApplicationResponse> RemoveProductAssetByIdAsync(int productId, int assetId);
        Task<ApplicationResponse> SetProductThumbnailAsync(int productId, int assetId);
        Task<ApplicationResponse> UpdateProductOfCurrentUserShopAsync(
            int productId,
            string? name,
            int? unitId,
            double? mass,
            double? pricePerMass,
            double? percentForFamiliarCustomer,
            double? percentForCustomer,
            double? priceForFamiliarCustomer,
            double? priceForCustomer,
            bool? hasAutoCalculatePermission,
            List<int>? categoryIds,
            List<Tuple<int, double>>? warehouses
        );
        Task<ApplicationResponse> AddPricePerMassOfCurrentUserShopAsync(List<int> categoryIds, double amountOfCash);
        Task<ApplicationResponse> SoftlyDeleteProductOfCurrentUserShopAsync(int id);
    }
}
