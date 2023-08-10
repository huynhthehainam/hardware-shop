
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Infrastructure.Services
{
    public class AssetService : IAssetService
    {
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        public AssetService(DbContext dbContext, IDistributedCache distributedCache) => (this.db, this.distributedCache) = (dbContext, distributedCache);

        public ApplicationResponse<CachedAssetDto> GetAssetById(long id)
        {
            var asset = db.GetCachedAssetById(distributedCache, id);
            if (asset == null)
            {

                return new ApplicationResponse<CachedAssetDto>
                {
                    Error = ApplicationError.CreateNotFoundError("Asset"),
                };

            }
            return new ApplicationResponse<CachedAssetDto> { Result = asset };
        }
    }
}