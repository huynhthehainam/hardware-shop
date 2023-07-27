using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public sealed class ProductAssetConfiguration : IEntityTypeConfiguration<ProductAsset>
    {


        public void Configure(EntityTypeBuilder<ProductAsset> p)
        {
            _ = p.HasQueryFilter(e => e.Product != null && !e.Product.IsDeleted);
            _ = p.HasKey(p => p.Id);
            _ = p.HasOne(e => e.Product).WithMany(e => e.ProductAssets).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            _ = p.HasOne(e => e.Asset).WithMany(e => e.ProductAssets).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade);


        }
    }
}