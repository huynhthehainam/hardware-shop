using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Enums;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public static class ContentTypeConstants
    {
        public const string JpegContentType = "image/jpeg";
        public const string PngContentType = "image/png";
    }


    public sealed class Asset : EntityBase, ITrackingDate
    {
        public Asset()
        {
        }

        public Asset(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public byte[]? Bytes { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public AssetProvider AssetProvider { get; set; } = AssetProvider.DbBlob;
        public string? StorageBucket { get; set; }
        public string? StorageObjectKey { get; set; }
        private ICollection<CountryAsset>? countryAssets;
        public ICollection<CountryAsset>? CountryAssets
        {
            get => lazyLoader?.Load(this, ref countryAssets);
            set => countryAssets = value;
        }


        private ICollection<ProductAsset>? productAssets;
        public ICollection<ProductAsset>? ProductAssets
        {
            get => lazyLoader?.Load(this, ref productAssets);
            set => productAssets = value;
        }
        private ICollection<ShopAsset>? shopAssets;
        public ICollection<ShopAsset>? ShopAssets
        {
            get => lazyLoader?.Load(this, ref shopAssets);
            set => shopAssets = value;
        }
        private ICollection<UserAsset>? userAssets;
        public ICollection<UserAsset>? UserAssets
        {
            get => lazyLoader?.Load(this, ref userAssets);
            set => userAssets = value;
        }

    }
}