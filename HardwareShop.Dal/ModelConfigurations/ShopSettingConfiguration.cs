using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ShopSettingConfiguration : ModelConfigurationBase<ShopSetting>
    {
        public ShopSettingConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = ss =>
             {
                 _ = ss.HasKey(s => s.ShopId);
                 _ = ss.HasOne(e => e.Shop).WithOne(e => e.ShopSetting).HasForeignKey<ShopSetting>(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
             };
        }
    }
}