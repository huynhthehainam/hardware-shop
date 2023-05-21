

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class CountryAssetConfiguration : ModelConfigurationBase<CountryAsset>
    {
        public CountryAssetConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                _ = e.HasKey(e => e.CountryId);
                _ = e.HasOne(e => e.Country).WithOne(e => e.Asset).HasForeignKey<CountryAsset>(e => e.CountryId).OnDelete(DeleteBehavior.Cascade);
                _ = e.HasOne(e => e.Asset).WithMany(e => e.CountryAssets).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}