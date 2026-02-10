
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;

namespace HardwareShop.Domain.Abstracts
{
    public enum AssetStorageProvider
    {
        DbBlob = 0,
        Minio = 1
    }

    public abstract class AssetEntityBase : EntityBase
    {
        public AssetEntityBase(Action<object, string?> lazyLoader) : base(lazyLoader) { }

        public AssetEntityBase() : base() { }
        public string AssetType { get; set; } = string.Empty;
        public Guid? AssetId { get; set; } = Guid.CreateVersion7();
        public AssetStorageProvider StorageProvider { get; set; } = AssetStorageProvider.DbBlob;
        public string? StorageBucket { get; set; }
        public string? StorageObjectKey { get; set; }
        private Asset? asset;
        public Asset? Asset
        {
            get => lazyLoader?.Load(this, ref asset);
            set => asset = value;
        }
    }

}
