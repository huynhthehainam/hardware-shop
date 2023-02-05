using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IShopService shopService;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<ProductAsset> productAssetRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        public ProductService(IShopService shopService, IRepository<Product> productRepository, IResponseResultBuilder responseResultBuilder, IRepository<ProductAsset> productAssetRepository)
        {
            this.shopService = shopService;
            this.productRepository = productRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.productAssetRepository = productAssetRepository;
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
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var productAsset = await productAssetRepository.GetItemByQueryAsync(e => e.ProductId == productId && (e.Product != null && e.Product.ShopId == shop.Id) && e.AssetType == ProductAssetConstants.ThumbnailAssetType);
            if (productAsset == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Asset");
                return null;
            }
            return productAsset;
        }
    }
}
