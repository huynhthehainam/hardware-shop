using HardwareShop.Business.Dtos;
using HardwareShop.Business.Extensions;
using HardwareShop.Business.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Extensions;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Business.Implementations
{
    public class CountryService : ICountryService
    {

        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        public CountryService(IDistributedCache distributedCache, DbContext db)
        {

            this.db = db;
            this.distributedCache = distributedCache;
        }

        public async Task<ApplicationResponse<CachedAsset>> GetCountryIconByIdAsync(int id)
        {
            var country = await db.Set<Country>().FirstOrDefaultAsync(e => e.Id == id);
            if (country == null)
            {
                return new ApplicationResponse<CachedAsset>
                {
                    Error = ApplicationError.CreateNotFoundError("Country")
                };

            }

            var asset = country.Asset;
            if (asset == null)
            {
                return new ApplicationResponse<CachedAsset>
                {
                    Error = ApplicationError.CreateNotFoundError("Asset")
                };

            }
            return new ApplicationResponse<CachedAsset>
            {
                Result = db.GetCachedAssetById(distributedCache, asset.AssetId)
            };
        }

        public async Task<ApplicationResponse<PageData<CountryDto>>> GetCountryPageData(PagingModel pagingModel, string? search)
        {

            var countryPageData = await db.Set<Country>().Where(e => true).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Country>(search, e => new { e.Name, e.PhonePrefix })).GetPageDataAsync(pagingModel);
            return new ApplicationResponse<PageData<CountryDto>>
            {
                Result = countryPageData.ConvertToOtherPageData(e => new CountryDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    PhonePrefix = e.PhonePrefix,
                })
            };
        }
    }
}