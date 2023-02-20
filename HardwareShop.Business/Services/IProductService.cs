using HardwareShop.Business.Dtos;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;

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
         bool hasAutoCalculatePermission
         );
        Task<IAssetTable?> GetProductThumbnail(int productId);
    }
}
