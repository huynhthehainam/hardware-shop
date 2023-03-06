

using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface IUnitService
    {
        Task<double?> RoundValue(int unitId, double value);
        Task<PageData<UnitDto>> GetUnitDtoPageDataAsync(PagingModel pagingModel, string? search, int? categoryId);
        Task<CreatedUnitDto?> CreateUnit(string name, double stepNumber, int unitCategoryId);

    }
}