using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public static class ProductAssetConstants
    {
        public const string ThumbnailAssetType = "thumbnail";
        public const string SlideAssetType = "slide";

    }

    public class ProductAsset : EntityBase, IAssetTable
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
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string Filename { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public string ContentType { get; set; } = string.Empty;

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ProductAsset>(p =>
            {
                _ = p.HasKey(p => p.Id);
                _ = p.HasOne(e => e.Product).WithMany(e => e.ProductAssets).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
