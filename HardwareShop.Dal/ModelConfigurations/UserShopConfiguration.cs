using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class UserShopConfiguration : ModelConfigurationBase<UserShop>
    {
        public UserShopConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = us =>
            {
                _ = us.HasKey(e => e.UserId);
                _ = us.HasOne(s => s.User).WithOne(e => e.UserShop).HasForeignKey<UserShop>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                _ = us.HasOne(s => s.Shop).WithMany(e => e.UserShops).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}