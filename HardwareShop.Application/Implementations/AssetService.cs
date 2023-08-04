
using HardwareShop.Application.Extensions;
using HardwareShop.Application.Services;
using HardwareShop.Core.Services;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Application.Implementations
{
    public class AssetService : IAssetService
    {
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;
        private readonly IResponseResultBuilder responseResultBuilder;
        public AssetService(DbContext dbContext, IDistributedCache distributedCache, IResponseResultBuilder responseResultBuilder) => (this.db, this.distributedCache, this.responseResultBuilder) = (dbContext, distributedCache, responseResultBuilder);

        public ApplicationResponse<CachedAsset> GetAssetById(long id)
        {
            var asset = db.GetCachedAssetById(distributedCache, id);
            if (asset == null)
            {

                return new ApplicationResponse<CachedAsset>
                {
                    Error = ApplicationError.CreateNotFoundError("Asset"),
                };

            }
            return new ApplicationResponse<CachedAsset> { Result = asset };
        }
    }
}