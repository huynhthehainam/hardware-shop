using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public class CountryAssetConfiguration : IEntityTypeConfiguration<CountryAsset>
    {

        public void Configure(EntityTypeBuilder<CountryAsset> e)
        {
            _ = e.HasKey(e => e.CountryId);
            _ = e.HasOne(e => e.Country).WithOne(e => e.Asset).HasForeignKey<CountryAsset>(e => e.CountryId).OnDelete(DeleteBehavior.Restrict);
            _ = e.HasOne(e => e.Asset).WithMany(e => e.CountryAssets).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}