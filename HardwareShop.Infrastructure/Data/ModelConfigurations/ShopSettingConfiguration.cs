using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class ShopSettingConfiguration : IEntityTypeConfiguration<ShopSetting>
    {
     
        public void Configure(EntityTypeBuilder<ShopSetting> ss)
        {
            _ = ss.HasKey(s => s.ShopId);
            _ = ss.HasOne(e => e.Shop).WithOne(e => e.ShopSetting).HasForeignKey<ShopSetting>(e => e.ShopId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}