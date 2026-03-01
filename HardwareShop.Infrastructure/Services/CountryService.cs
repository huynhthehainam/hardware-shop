using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Infrastructure.Services
{
    public class CountryService(DbContext db, IDistributedCache distributedCache, IAssetService assetService) : ICountryService
    {


        public async Task<ApplicationResponse<CachedAssetDto>> GetCountryIconByIdAsync(Guid id)
        {
            var country = await db.Set<Country>().FirstOrDefaultAsync(e => e.Id == id);
            if (country == null)
            {
                return new ApplicationResponse<CachedAssetDto>
                (ApplicationError.CreateNotFoundError("Country"));

            }

            var asset = country.Asset;
            if (asset == null)
            {
                return new ApplicationResponse<CachedAssetDto>
              (ApplicationError.CreateNotFoundError("Asset"));

            }
            return new ApplicationResponse<CachedAssetDto>
          ((await assetService.GetAssetByIdAsync(asset.AssetId)).ExtractResult());
        }

        public async Task<ApplicationResponse<PageData<CountryDto>>> GetCountryPageData(PagingModel pagingModel, string? search)
        {

            var countryPageData = await db.Set<Country>().Where(e => true).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Country>(search, e => new { e.Name, e.PhonePrefix })).GetPageDataAsync(pagingModel);

            return new ApplicationResponse<PageData<CountryDto>>(countryPageData.ConvertToOtherPageData(e => new CountryDto
            {
                Id = e.Id,
                Name = e.Name,
                PhonePrefix = e.PhonePrefix,
            }));
        }
    }
}