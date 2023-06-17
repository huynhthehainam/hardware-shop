
using HardwareShop.Business.Extensions;
using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Extensions;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Business.Implementations
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