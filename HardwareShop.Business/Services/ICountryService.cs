


using HardwareShop.Business.Dtos;
using HardwareShop.Business.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Services
{
    public interface ICountryService
    {
        Task<ApplicationResponse<CachedAsset>> GetCountryIconByIdAsync(int id);

        Task<ApplicationResponse<PageData<CountryDto>>> GetCountryPageData(PagingModel pagingModel, string? search);
    }
}