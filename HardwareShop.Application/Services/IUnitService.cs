

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IUnitService
    {
        Task<ApplicationResponse<double>> RoundValue(int unitId, double value);
        Task<PageData<UnitDto>> GetUnitDtoPageDataAsync(PagingModel pagingModel, string? search, int? categoryId);
        Task<ApplicationResponse<CreatedUnitDto>> CreateUnitAsync(CreateUnitDto model);
        bool IsCashUnitExist(int cashUnitId);
    }
}