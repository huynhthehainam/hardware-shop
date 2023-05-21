using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Dal.Extensions
{
    public static class ResponseResultBuilderAssetExtensions
    {
        public static void SetAsset(this IResponseResultBuilder responseResultBuilder, CachedAsset cachedAsset)
        {
            responseResultBuilder.SetFile(cachedAsset.Bytes, cachedAsset.ContentType, cachedAsset.Filename);
        }
    }
}