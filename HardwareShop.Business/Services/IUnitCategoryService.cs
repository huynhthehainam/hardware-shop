

using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface IUnitCategoryService
    {
        Task<PageData<UnitCategoryDto>> GetUnitCategoryPageDataAsync(PagingModel pagingModel, string? search);
    }
}