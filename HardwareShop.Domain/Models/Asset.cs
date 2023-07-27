using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{
    public static class ContentTypeConstants
    {
        public const string JpegContentType = "image/jpeg";
        public const string PngContentType = "image/png";
    }

    public abstract class AssetEntityBase : EntityBase
    {
        public AssetEntityBase(ILazyLoader lazyLoader) : base(lazyLoader) { }

        public AssetEntityBase() : base() { }
        public string AssetType { get; set; } = string.Empty;
        public long AssetId { get; set; }
        private Asset? asset;
        public Asset? Asset
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref asset) : asset;
            set => asset = value;
        }
    }
    public sealed class CachedAsset
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string Filename { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public static CachedAsset BuildFromAsset(Asset asset)
        {
            return new CachedAsset()
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
    public sealed class Asset : EntityBase, ITrackingDate
    {
        public Asset()
        {
        }

        public Asset(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string Filename { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

        private ICollection<CountryAsset>? countryAssets;
        public ICollection<CountryAsset>? CountryAssets
        {
            get => lazyLoader != null ? lazyLoader.Load(this, ref countryAssets) : countryAssets;
            set => countryAssets = value;
        }
        private ICollection<ProductAsset>? productAssets;
        public ICollection<ProductAsset>? ProductAssets
        {
            get => lazyLoader != null ? lazyLoader.Load(this, ref productAssets) : productAssets;
            set => productAssets = value;
        }
        private ICollection<ShopAsset>? shopAssets;
        public ICollection<ShopAsset>? ShopAssets
        {
            get => lazyLoader != null ? lazyLoader.Load(this, ref shopAssets) : shopAssets;
            set => shopAssets = value;
        }
        private ICollection<UserAsset>? userAssets;
        public ICollection<UserAsset>? UserAssets
        {
            get => lazyLoader != null ? lazyLoader.Load(this, ref userAssets) : userAssets;
            set => userAssets = value;
        }
        private ICollection<ChatSession>? chatSessions;
        public ICollection<ChatSession>? ChatSessions
        {
            get => lazyLoader?.Load(this, ref chatSessions);
            set => chatSessions = value;
        }
    }
}