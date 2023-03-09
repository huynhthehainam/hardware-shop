


using HardwareShop.Business.Dtos;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface ICountryService
    {
        Task<IAssetTable?> GetCountryIconByIdAsync(int id);
        
        Task<PageData<CountryDto>> GetCountryPageData(PagingModel pagingModel, string? search);
    }
}