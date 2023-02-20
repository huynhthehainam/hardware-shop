using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IShopService shopService;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<ProductAsset> productAssetRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IRepository<Unit> unitRepository;
        public ProductService(IRepository<Unit> unitRepository, IShopService shopService, IRepository<Product> productRepository, IResponseResultBuilder responseResultBuilder, IRepository<ProductAsset> productAssetRepository)
        {
            this.unitRepository = unitRepository;
            this.shopService = shopService;
            this.productRepository = productRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.productAssetRepository = productAssetRepository;
        }

        public async Task<CreatedProductDto?> CreateProductOfShopAsync(string name, int unitId, double? mass, double? pricePerMass, double? percentForFamiliarCustomer, double? percentForCustomer, double? priceForFamiliarCustomer, double priceForCustomer, bool hasAutoCalculatePermission)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var unit = await unitRepository.GetItemByQueryAsync(e => e.Id == unitId);
            if (unit == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Unit");
                return null;
            }
          
            var product = await productRepository.CreateIfNotExistsAsync(new Product
            {
                Name = name,
                PricePerMass = pricePerMass,
                PercentForFamiliarCustomer = percentForFamiliarCustomer,
                PercentForCustomer = percentForCustomer,
                PriceForFamiliarCustomer = priceForFamiliarCustomer,
                PriceForCustomer = priceForCustomer,
                ShopId = shop.Id,
                UnitId = unit.Id,
                HasAutoCalculatePermission = hasAutoCalculatePermission,
            }, e => new
            {
                e.Name
            });
            if (product == null)
            {
                responseResultBuilder.AddInvalidFieldError("Name");
                return null;
            }
            return new CreatedProductDto { Id = product.Id };
        }

        public Task<CreatedProductDto?> CreateProductOfShopAsync(string name, int unitId, double? mass, double? pricePerMass, double? percentForFamiliarCustomer, double? percentForCustomer, double? priceForFamiliarCustomer, double priceForCustomer, int priceUnitId, bool hasAutoCalculatePermission)
        {
            throw new NotImplementedException();
        }

        public async Task<PageData<ProductDto>?> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            PageData<Product> productPageData = await productRepository.GetPageDataByQueryAsync(pagingModel, e => (e.ShopId == shop.Id), string.IsNullOrEmpty(search) ? null : new SearchQuery<Product>(search, e => new
            {
                e.Name
            }));

            return PageData<ProductDto>.ConvertFromOtherPageData(productPageData, e => new ProductDto
            {
                Name = e.Name,
                Id = e.Id,
                Mass = e.Mass,
                PercentForCustomer = e.PercentForCustomer,
                PercentForFamiliarCustomer = e.PercentForFamiliarCustomer,
                PriceForCustomer = e.PriceForCustomer,
                PriceForFamiliarCustomer = e.PriceForFamiliarCustomer,
                PricePerMass = e.PricePerMass,
                ProductCategoryIds = e.ProductCategoryProducts != null ? e.ProductCategoryProducts.Select(e => e.ProductCategoryId).ToArray() : new int[0],
                ProductCategoryNames = e.ProductCategoryProducts != null ? e.ProductCategoryProducts.Select(e => e.ProductCategory?.Name).ToArray() : new string[0],
                UnitId = e.UnitId,
                UnitName = e.Unit?.Name

            });

        }

        public async Task<IAssetTable?> GetProductThumbnail(int productId)
        {

            var productAsset = await productAssetRepository.GetItemByQueryAsync(e => e.ProductId == productId && (e.Product != null) && e.AssetType == ProductAssetConstants.ThumbnailAssetType);
            if (productAsset == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Asset");
                return null;
            }
            return productAsset;
        }
    }
}
