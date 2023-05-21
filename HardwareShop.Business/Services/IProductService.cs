using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Business.Services
{
    public interface IProductService
    {
        Task<PageData<ProductDto>?> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search, SortingModel sortingModel);
        Task<CreatedProductDto?> CreateProductOfShopAsync(string name, int unitId,
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
        Task<CachedAsset?> GetProductThumbnailAsync(int productId);
        Task<CachedAsset?> GetProductAssetByIdAsync(int productId, int assetId);
        Task<int?> UploadProductImageOfCurrentUserShopAsync(int productId, string assetType, IFormFile file);
        Task<ProductDto?> GetProductDtoOfCurrentUserShopAsync(int productId);
        Task<bool> RemoveProductAssetByIdAsync(int productId, int assetId);
        Task<bool> SetProductThumbnailAsync(int productId, int assetId);
        Task<bool> UpdateProductOfCurrentUserShopAsync(
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
        Task<bool> AddPricePerMassOfCurrentUserShopAsync(List<int> categoryIds, double amountOfCash);
        Task<bool> SoftlyDeleteProductOfCurrentUserShopAsync(int id);
    }
}
