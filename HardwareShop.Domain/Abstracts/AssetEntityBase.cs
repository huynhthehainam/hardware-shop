
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;

namespace HardwareShop.Domain.Abstracts
{
    public abstract class AssetEntityBase : EntityBase
    {
        public AssetEntityBase(Action<object, string?> lazyLoader) : base(lazyLoader) { }

        public AssetEntityBase() : base() { }
        public string AssetType { get; set; } = string.Empty;
        public Guid AssetId { get; set; } = Guid.CreateVersion7();
        private Asset? asset;
        public Asset? Asset
        {
            get => lazyLoader?.Load(this, ref asset);
            set => asset = value;
        }
    }

}