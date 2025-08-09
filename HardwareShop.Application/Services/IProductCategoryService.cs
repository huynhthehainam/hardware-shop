using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IProductCategoryService
    {
        Task<ApplicationResponse<PageData<ProductCategoryDto>>> GetCategoryPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<ApplicationResponse<ProductCategoryDto>> CreateCategoryOfCurrentUserShopAsync(string name, string? description);
    }

}