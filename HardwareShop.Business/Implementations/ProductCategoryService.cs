



using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IRepository<ProductCategory> productCategoryRepository;
        private readonly IShopService shopService;
        private readonly IResponseResultBuilder responseResultBuilder;
        public ProductCategoryService(IResponseResultBuilder responseResultBuilder, IShopService shopService, IRepository<ProductCategory> productCategoryRepository)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.productCategoryRepository = productCategoryRepository;
            this.shopService = shopService;
        }

        public async Task<ProductCategoryDto?> CreateCategoryOfCurrentUserShopAsync(string name, string? description)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            ProductCategory category = await productCategoryRepository.CreateAsync(new ProductCategory { ShopId = shop.Id, Name = name, Description = description });
            return new ProductCategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task<PageData<ProductCategoryDto>?> GetCategoryPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            PageData<ProductCategory> categories = await productCategoryRepository.GetPageDataByQueryAsync(pagingModel,
            e => e.ShopId == shop.Id, string.IsNullOrEmpty(search) ? null : new SearchQuery<ProductCategory>(search, e => new { e.Name }), new List<QueryOrder<ProductCategory>> { new QueryOrder<ProductCategory>(e => e.Name, true) });
            return PageData<ProductCategoryDto>.ConvertFromOtherPageData(categories, e => new ProductCategoryDto
            {
                Id = e.Id,
                Name = e.Name
            });
        }
    }
}