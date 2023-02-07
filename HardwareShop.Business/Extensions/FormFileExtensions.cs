using HardwareShop.Core.Bases;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Business.Extensions
{
    public static class FormFileExtensions
    {
        public static T ConvertToAsset<T>(this IFormFile file, T asset) where T : IAssetTable
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                asset.Bytes = fileBytes;
            }
            asset.ContentType = file.ContentType;
            asset.Filename = file.FileName;
            return asset;
        }
    }
}
