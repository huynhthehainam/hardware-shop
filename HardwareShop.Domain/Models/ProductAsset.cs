using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public static class ProductAssetConstants
    {
        public const string ThumbnailAssetType = "thumbnail";
        public const string SlideAssetType = "slide";

    }

    public class ProductAsset : AssetEntityBase
    {
        public ProductAsset()
        {
        }

        public ProductAsset(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader?.Load(this, ref product);
            set => product = value;
        }

    }
}
