using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public sealed class UserAssetConfiguration : IEntityTypeConfiguration<UserAsset>
    {


        public void Configure(EntityTypeBuilder<UserAsset> ua)
        {
            _ = ua.HasKey(e => e.Id);
            _ = ua.HasOne(e => e.User).WithMany(e => e.Assets).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            _ = ua.HasOne(e => e.Asset).WithMany(e => e.UserAssets).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade);


        }
    }
}