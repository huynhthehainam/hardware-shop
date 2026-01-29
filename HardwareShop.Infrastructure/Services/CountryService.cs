using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Infrastructure.Services
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

        public async Task<ApplicationResponse<CachedAssetDto>> GetCountryIconByIdAsync(Guid id)
        {
            var country = await db.Set<Country>().FirstOrDefaultAsync(e => e.Id == id);
            if (country == null)
            {
                return new ApplicationResponse<CachedAssetDto>
                {
                    Error = ApplicationError.CreateNotFoundError("Country")
                };

            }

            var asset = country.Asset;
            if (asset == null)
            {
                return new ApplicationResponse<CachedAssetDto>
                {
                    Error = ApplicationError.CreateNotFoundError("Asset")
                };

            }
            return new ApplicationResponse<CachedAssetDto>
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