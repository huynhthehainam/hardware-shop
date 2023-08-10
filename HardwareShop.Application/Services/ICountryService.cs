


using HardwareShop.Application.Dtos;
using HardwareShop.Application.Extensions;
using HardwareShop.Application.Models;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface ICountryService
    {
        Task<ApplicationResponse<CachedAssetDto>> GetCountryIconByIdAsync(int id);

        Task<ApplicationResponse<PageData<CountryDto>>> GetCountryPageData(PagingModel pagingModel, string? search);
    }
}