

using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country> countryRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        public CountryService(IRepository<Country> countryRepository, IResponseResultBuilder responseResultBuilder)
        {
            this.countryRepository = countryRepository;
            this.responseResultBuilder = responseResultBuilder;
        }

        public async Task<IAssetTable?> GetCountryIconByIdAsync(int id)
        {
            var country = await countryRepository.GetItemByQueryAsync(e => e.Id == id);
            if (country == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Country");
                return null;
            }

            var asset = country.Asset;
            if (asset == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Asset");
                return null;
            }
            return asset;
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