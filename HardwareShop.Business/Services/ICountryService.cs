


using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface ICountryService
    {
        Task<PageData<CountryDto>> GetCountryPageData(PagingModel pagingModel, string? search);
    }
}