

using HardwareShop.Application.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IUnitCategoryService
    {
        Task<PageData<UnitCategoryDto>> GetUnitCategoryPageDataAsync(PagingModel pagingModel, string? search);
    }
}