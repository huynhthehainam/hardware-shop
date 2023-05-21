


using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Services
{
    public interface ICountryService
    {
        Task<CachedAsset?> GetCountryIconByIdAsync(int id);
        
        Task<PageData<CountryDto>> GetCountryPageData(PagingModel pagingModel, string? search);
    }
}