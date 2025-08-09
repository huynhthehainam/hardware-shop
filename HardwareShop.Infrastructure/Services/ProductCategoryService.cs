



using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using HardwareShop.Application.Models;
using HardwareShop.Infrastructure.Extensions;

namespace HardwareShop.Infrastructure.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IShopService shopService;
        private readonly DbContext db;
        public ProductCategoryService(IShopService shopService, DbContext db)
        {
            this.shopService = shopService;
            this.db = db;
        }

        public async Task<ApplicationResponse<ProductCategoryDto>> CreateCategoryOfCurrentUserShopAsync(string name, string? description)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            ProductCategory category = new() { ShopId = shop.Id, Name = name, Description = description };
            db.Add(category);
            db.SaveChanges();
            return new(new ProductCategoryDto { Id = category.Id, Name = category.Name });
        }

        public async Task<ApplicationResponse<PageData<ProductCategoryDto>>> GetCategoryPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            var categoryPageData = db.Set<ProductCategory>().Where(e => e.ShopId == shop.Id).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<ProductCategory>(search, e => new { e.Name })).GetPageData(pagingModel);


            return new(categoryPageData.ConvertToOtherPageData(e => new ProductCategoryDto
            {
                Id = e.Id,
                Name = e.Name
            }));
        }
    }
}