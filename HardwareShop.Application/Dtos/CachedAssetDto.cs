using HardwareShop.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Application.Dtos;
public sealed class CachedAssetDto
{
    public long Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
    public byte[] Bytes { get; set; } = Array.Empty<byte>();
    public string Filename { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public static CachedAssetDto BuildFromAsset(Asset asset)
    {
        return new CachedAssetDto()
        {
            Bytes = asset.Bytes,
            ContentType = asset.ContentType,
            CreatedDate = asset.CreatedDate,
            Filename = asset.Filename,
            Id = asset.Id,
            LastModifiedDate = asset.LastModifiedDate,
        };
    }
}
