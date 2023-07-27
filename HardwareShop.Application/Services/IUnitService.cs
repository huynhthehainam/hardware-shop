

using HardwareShop.Application.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IUnitService
    {
        Task<double?> RoundValue(int unitId, double value);
        Task<PageData<UnitDto>> GetUnitDtoPageDataAsync(PagingModel pagingModel, string? search, int? categoryId);
        Task<CreatedUnitDto?> CreateUnitAsync(CreateUnitDto model);
        bool IsCashUnitExist(int cashUnitId);
    }
}