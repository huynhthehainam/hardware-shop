using HardwareShop.Application.Dtos;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Infrastructure.Extensions
{
    public static class AssetDtoExtensions
    {
        public static T ConvertToAsset<T>(this AssetDto file, T assetEntityBase) where T : AssetEntityBase
        {
            var asset = assetEntityBase.Asset;
            if (asset == null)
            {
                asset = new Asset()
                {

                };
            }
            if (asset == null) return assetEntityBase;

            asset.Bytes = file.Bytes;
            asset.ContentType = file.ContentType;
            asset.FileName = file.FileName;
            assetEntityBase.Asset = asset;
            return assetEntityBase;
        }
    }
}
