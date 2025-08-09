
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;

namespace HardwareShop.Domain.Abstracts
{
    public abstract class AssetEntityBase : EntityBase
    {
        public AssetEntityBase(Action<object, string?> lazyLoader) : base(lazyLoader) { }

        public AssetEntityBase() : base() { }
        public string AssetType { get; set; } = string.Empty;
        public long AssetId { get; set; }
        private Asset? asset;
        public Asset? Asset
        {
            get => lazyLoader?.Load(this, ref asset);
            set => asset = value;
        }
    }

}