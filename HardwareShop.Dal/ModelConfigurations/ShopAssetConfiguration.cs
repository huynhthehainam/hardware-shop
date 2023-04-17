using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ShopAssetConfiguration : ModelConfigurationBase<ShopAsset>
    {
        public ShopAssetConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = s =>
            {
                _ = s.HasKey(s => s.Id);
                _ = s.HasOne(e => e.Shop).WithMany(e => e.Assets).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}