using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Extensions;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Business.Implementations
{
    public class CountryService : ICountryService
    {

        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        public CountryService(IResponseResultBuilder responseResultBuilder, IDistributedCache distributedCache, DbContext db)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.db = db;
            this.distributedCache = distributedCache;
        }

        public async Task<CachedAsset?> GetCountryIconByIdAsync(int id)
        {
            var country = await db.Set<Country>().FirstOrDefaultAsync(e => e.Id == id);
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
            return db.GetCachedAssetById(distributedCache, asset.AssetId);
        }

        public async Task<PageData<CountryDto>> GetCountryPageData(PagingModel pagingModel, string? search)
        {

            var countryPageData = await db.Set<Country>().Where(e => true).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Country>(search, e => new { e.Name, e.PhonePrefix })).GetPageDataAsync(pagingModel);
            return countryPageData.ConvertToOtherPageData(e => new CountryDto
            {
                Id = e.Id,
                Name = e.Name,
                PhonePrefix = e.PhonePrefix,
            });
        }
    }
}