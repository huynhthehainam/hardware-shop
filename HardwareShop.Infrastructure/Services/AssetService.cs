
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Infrastructure.Services
{
    public static class AssetCacheHelpers
    {
        private const string contentKeyPrefix = "content$_";
        private const string contentTypeKeyPrefix = "contentTyPPE$_";
        private const string fileNameKeyPrefix = "fIleNAAme$_";
        private const string createdDateKeyPrefix = "creatED$_";
        private const string modifiedDateKeyPrefix = "mmaodified$_";
        public static Tuple<string, string, string, string, string> GetAssetCacheKeys(Guid id)
        {
            return new Tuple<string, string, string, string, string>($"{contentKeyPrefix}_{id}", $"{contentTypeKeyPrefix}_{id}", $"{fileNameKeyPrefix}_{id}", $"{createdDateKeyPrefix}_{id}", $"{modifiedDateKeyPrefix}_{id}");
        }
    }
    public class AssetService(DbContext db, IDistributedCache distributedCache, IMinioService minioService) : IAssetService
    {


        public async Task<ApplicationResponse<CachedAssetDto>> GetAssetByIdAsync(Guid id)
        {
            var asset = await GetCachedAssetByIdAsync(id);
            if (asset == null)
            {

                return new ApplicationResponse<CachedAssetDto>
              (ApplicationError.CreateNotFoundError("Asset"));

            }
            return new ApplicationResponse<CachedAssetDto>(asset);
        }

        private async Task<CachedAssetDto?> GetCachedAssetByIdAsync(Guid id)
        {
            var assetSet = db.Set<Asset>();
            var keys = AssetCacheHelpers.GetAssetCacheKeys(id);
            byte[]? content = distributedCache.Get(keys.Item1);
            if (content == null)
            {
                var asset = assetSet.FirstOrDefault(e => e.Id == id);
                if (asset != null)
                {
                    byte[]? fileBytes = null;
                    if (asset.AssetProvider == Domain.Enums.AssetProvider.DbBlob)
                    {
                        fileBytes = asset.Bytes;
                    }
                    else if (asset.AssetProvider == Domain.Enums.AssetProvider.Minio)
                    {
                        fileBytes = await minioService.GetFileBytesByKeyAsync(asset.StorageBucket ?? string.Empty, asset.StorageObjectKey ?? string.Empty);
                    }
                    if (fileBytes is null)
                    {
                        throw new Exception();
                    }
                    return SaveAssetToCache(distributedCache, asset, fileBytes);
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
        private static DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0, 5, 0)
        };
        private CachedAssetDto SaveAssetToCache(IDistributedCache distributedCache, Asset asset, byte[] fileBytes)
        {
            var keys = AssetCacheHelpers.GetAssetCacheKeys(asset.Id);
            distributedCache.Set(keys.Item1, fileBytes, cacheEntryOptions);
            distributedCache.SetString(keys.Item2, asset.ContentType, cacheEntryOptions);
            distributedCache.SetString(keys.Item3, asset.FileName, cacheEntryOptions);
            distributedCache.SetString(keys.Item4, asset.CreatedDate.ToString(), cacheEntryOptions);
            distributedCache.SetString(keys.Item5, asset.LastModifiedDate?.ToString() ?? "", cacheEntryOptions);
            return CachedAssetDto.BuildFromAsset(asset, fileBytes);
        }
    }
}