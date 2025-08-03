using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class ShopAssetConfiguration : IEntityTypeConfiguration<ShopAsset>
    {
      

        public void Configure(EntityTypeBuilder<ShopAsset> s)
        {
            _ = s.HasKey(s => s.Id);
            _ = s.HasOne(e => e.Shop).WithMany(e => e.Assets).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Restrict);
            _ = s.HasOne(e => e.Asset).WithMany(e => e.ShopAssets).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Restrict);


        }
    }
}