using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.Data.ModelConfigurations
{
    public sealed class ProductAssetConfiguration : IEntityTypeConfiguration<ProductAsset>
    {


        public void Configure(EntityTypeBuilder<ProductAsset> p)
        {
            _ = p.HasKey(p => p.Id);
            _ = p.HasOne(e => e.Product).WithMany(e => e.ProductAssets).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
            _ = p.HasOne(e => e.Asset).WithMany(e => e.ProductAssets).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Restrict);


        }
    }
}