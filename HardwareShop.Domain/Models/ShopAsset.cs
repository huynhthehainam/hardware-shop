using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public static class ShopAssetConstants
    {
        public const string LogoAssetType = "logo";
    }
    public sealed class ShopAsset : AssetEntityBase
    {
        public ShopAsset()
        {
        }

        public ShopAsset(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get =>  lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }
    }
}
