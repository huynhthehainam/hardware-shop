using HardwareShop.Business.Dtos;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Business.Services
{
    public interface IProductService
    {
        Task<PageData<ProductDto>?> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
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
        Task<IAssetTable?> GetProductThumbnailAsync(int productId);
        Task<IAssetTable?> GetProductAssetByIdAsync(int productId, int assetId);
        Task<int?> UploadProductImageOfCurrentUserShopAsync(int productId, string assetType, IFormFile file);
        Task<ProductDto?> GetProductOrCurrentUserShopAsync(int productId);
    }
}
