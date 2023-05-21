using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Dal.Repositories
{
    public interface IAssetRepository : IRepository<Asset>
    {
        Task<CachedAsset?> GetCachedAssetByIdAsync(long id);
        Task<CachedAsset?> GetCachedAssetFromAssetEntityBaseAsync(AssetEntityBase asset);
    }
    public class AssetRepository : RepositoryBase<Asset>, IAssetRepository
    {
        private const string contentKeyPrefix = "content$_";
        private const string contentTypeKeyPrefix = "contentTyPPE$_";
        private const string fileNameKeyPrefix = "fIleNAAme$_";
        private const string createdDateKeyPrefix = "creatED$_";
        private const string modifiedDateKeyPrefix = "mmaodified$_";
        private readonly IDistributedCache distributedCache;
        private static DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0, 5, 0)
        };
        public AssetRepository(Microsoft.EntityFrameworkCore.DbContext db, IDistributedCache distributedCache) : base(db)
        {
            this.distributedCache = distributedCache;
        }
        private Tuple<string, string, string, string, string> GetAssetCacheKeys(long id)
        {
            return new Tuple<string, string, string, string, string>($"{contentKeyPrefix}_{id}", $"{contentTypeKeyPrefix}_{id}", $"{fileNameKeyPrefix}_{id}", $"{createdDateKeyPrefix}_{id}", $"{modifiedDateKeyPrefix}_{id}");
        }

        private CachedAsset SaveAssetToCache(Asset asset)
        {
            var keys = GetAssetCacheKeys(asset.Id);
            distributedCache.Set(keys.Item1, asset.Bytes, cacheEntryOptions);
            distributedCache.SetString(keys.Item2, asset.ContentType, cacheEntryOptions);
            distributedCache.SetString(keys.Item3, asset.Filename, cacheEntryOptions);
            distributedCache.SetString(keys.Item4, asset.CreatedDate.ToString(), cacheEntryOptions);
            distributedCache.SetString(keys.Item5, asset.LastModifiedDate?.ToString() ?? "", cacheEntryOptions);
            return CachedAsset.BuildFromAsset(asset);
        }
        public async Task<CachedAsset?> GetCachedAssetFromAssetEntityBaseAsync(AssetEntityBase asset)
        {
            return await GetCachedAssetByIdAsync(asset.AssetId);
        }

        public async Task<CachedAsset?> GetCachedAssetByIdAsync(long id)
        {
            var keys = GetAssetCacheKeys(id);
            byte[]? content = distributedCache.Get(keys.Item1);
            if (content == null)
            {
                var asset = await GetItemByQueryAsync(e => e.Id == id);
                if (asset != null)
                {
                    return SaveAssetToCache(asset);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string? contentType = distributedCache.GetString(keys.Item2);
                string? fileName = distributedCache.GetString(keys.Item3);
                string? createdDate = distributedCache.GetString(keys.Item4);
                string? modifiedDate = distributedCache.GetString(keys.Item5);
                return new CachedAsset()
                {
                    Bytes = content,
                    Filename = fileName ?? "",
                    ContentType = contentType ?? "",
                    CreatedDate = createdDate != null ? DateTime.Parse(createdDate) : DateTime.UtcNow,
                    LastModifiedDate = modifiedDate != null ? DateTime.Parse(modifiedDate) : null,
                    Id = id,
                };
            }
        }

    }
}