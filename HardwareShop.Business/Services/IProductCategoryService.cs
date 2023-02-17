using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface IProductCategoryService
    {
        Task<PageData<ProductCategoryDto>?> GetCategoryPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<ProductCategoryDto?> CreateCategoryOfCurrentUserShopAsync(string name, string? description);
    }

}