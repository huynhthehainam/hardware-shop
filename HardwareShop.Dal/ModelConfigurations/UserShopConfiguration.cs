using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class UserShopConfiguration : IEntityTypeConfiguration<UserShop>
    {


        public void Configure(EntityTypeBuilder<UserShop> us)
        {
            _ = us.HasKey(e => e.UserId);
            _ = us.HasOne(s => s.User).WithOne(e => e.UserShop).HasForeignKey<UserShop>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            _ = us.HasOne(s => s.Shop).WithMany(e => e.UserShops).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}