
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

        public CachedAsset? GetAssetById(long id)
        {
            var asset = db.GetCachedAssetById(distributedCache, id);
            if (asset == null)
            {

                responseResultBuilder.AddNotFoundEntityError("Asset");
                return null;
            }
            return asset;
        }
    }
}