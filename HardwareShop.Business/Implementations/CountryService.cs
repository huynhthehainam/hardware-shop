

using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country> countryRepository;
        public CountryService(IRepository<Country> countryRepository)
        {
            this.countryRepository = countryRepository;
        }

        public async Task<PageData<CountryDto>> GetCountryPageData(PagingModel pagingModel, string? search)
        {
            return await countryRepository.GetDtoPageDataByQueryAsync<CountryDto>(pagingModel, e => true, e => new CountryDto
            {
                Id = e.Id,
                Name = e.Name,
                PhonePrefix = e.PhonePrefix,
            }, string.IsNullOrEmpty(search) ? null : new SearchQuery<Country>(search, e => new { e.Name, e.PhonePrefix }));
        }
    }
}