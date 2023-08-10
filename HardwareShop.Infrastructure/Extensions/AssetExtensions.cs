

using HardwareShop.Application.Dtos;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Infrastructure.Extensions
{
    public static class AssetCacheHelpers
    {
        private const string contentKeyPrefix = "content$_";
        private const string contentTypeKeyPrefix = "contentTyPPE$_";
        private const string fileNameKeyPrefix = "fIleNAAme$_";
        private const string createdDateKeyPrefix = "creatED$_";
        private const string modifiedDateKeyPrefix = "mmaodified$_";
        public static Tuple<string, string, string, string, string> GetAssetCacheKeys(long id)
        {
            return new Tuple<string, string, string, string, string>($"{contentKeyPrefix}_{id}", $"{contentTypeKeyPrefix}_{id}", $"{fileNameKeyPrefix}_{id}", $"{createdDateKeyPrefix}_{id}", $"{modifiedDateKeyPrefix}_{id}");
        }
    }
    public static class AssetExtensions
    {
        private static DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0, 5, 0)
        };
        private static CachedAssetDto SaveAssetToCache(IDistributedCache distributedCache, Asset asset)
        {
            var keys = AssetCacheHelpers.GetAssetCacheKeys(asset.Id);
            distributedCache.Set(keys.Item1, asset.Bytes, cacheEntryOptions);
            distributedCache.SetString(keys.Item2, asset.ContentType, cacheEntryOptions);
            distributedCache.SetString(keys.Item3, asset.FileName, cacheEntryOptions);
            distributedCache.SetString(keys.Item4, asset.CreatedDate.ToString(), cacheEntryOptions);
            distributedCache.SetString(keys.Item5, asset.LastModifiedDate?.ToString() ?? "", cacheEntryOptions);
            return CachedAssetDto.BuildFromAsset(asset);
        }
        public static CachedAssetDto? GetCachedAssetById(this DbContext db, IDistributedCache distributedCache, long id)
        {
            var assetSet = db.Set<Asset>();
            var keys = AssetCacheHelpers.GetAssetCacheKeys(id);
            byte[]? content = distributedCache.Get(keys.Item1);
            if (content == null)
            {
                var asset = assetSet.FirstOrDefault(e => e.Id == id);
                if (asset != null)
                {
                    return SaveAssetToCache(distributedCache, asset);
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
                return new CachedAssetDto()
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