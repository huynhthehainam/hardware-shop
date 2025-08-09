using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Models;

namespace HardwareShop.Infrastructure.Extensions
{
    public static class AssetEntityBaseExtensions
    {
        public static string ConvertToImgSrc(this AssetEntityBase entity)
        {
            var asset = entity.Asset;
            if (asset == null) return "";
            return $"data:{asset.ContentType};base64,{Convert.ToBase64String(asset.Bytes)}";
        }
    }
}