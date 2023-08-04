using HardwareShop.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Domain.Extensions
{
    public static class FormFileExtensions
    {
        public static T ConvertToAsset<T>(this IFormFile file, T assetEntityBase) where T : AssetEntityBase
        {
            var asset = assetEntityBase.Asset;
            if (asset == null)
            {
                asset = new Asset()
                {

                };
            }
            if (asset == null) return assetEntityBase;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                asset.Bytes = fileBytes;
            }
            asset.ContentType = file.ContentType;
            asset.Filename = file.FileName;
            assetEntityBase.Asset = asset;
            return assetEntityBase;
        }
    }
}
