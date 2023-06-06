



using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Business.Implementations
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IShopService shopService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly DbContext db;
        public ProductCategoryService(IResponseResultBuilder responseResultBuilder, IShopService shopService, DbContext db)
        {
            this.responseResultBuilder = responseResultBuilder;

            this.shopService = shopService;
            this.db = db;
        }

        public async Task<ProductCategoryDto?> CreateCategoryOfCurrentUserShopAsync(string name, string? description)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            ProductCategory category = new ProductCategory { ShopId = shop.Id, Name = name, Description = description };
            db.Add(category);
            db.SaveChanges();
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
            var categoryPageData = db.Set<ProductCategory>().Where(e => e.ShopId == shop.Id).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<ProductCategory>(search, e => new { e.Name })).GetPageData(pagingModel);


            return categoryPageData.ConvertToOtherPageData(e => new ProductCategoryDto
            {
                Id = e.Id,
                Name = e.Name
            });
        }
    }
}