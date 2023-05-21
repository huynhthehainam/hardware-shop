using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
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

        public ProductAsset(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader != null ? lazyLoader.Load(this, ref product) : product;
            set => product = value;
        }

    }
}
