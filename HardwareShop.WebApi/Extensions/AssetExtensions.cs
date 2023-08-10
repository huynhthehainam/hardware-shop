
using HardwareShop.Application.Dtos;

namespace HardwareShop.WebApi.Extensions
{
    public static class AssetExtensions
    {
        public static AssetDto ConvertToAsset(this IFormFile file)
        {
            var asset = new AssetDto();
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                asset.Bytes = fileBytes;
            }
            asset.ContentType = file.ContentType;
            asset.FileName = file.FileName;
            return asset;
        }
    }
}