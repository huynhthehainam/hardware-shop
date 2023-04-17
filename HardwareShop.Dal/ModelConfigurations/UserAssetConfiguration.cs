using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class UserAssetConfiguration : ModelConfigurationBase<UserAsset>
    {
        public UserAssetConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = ua =>
            {
                _ = ua.HasKey(e => e.Id);
                _ = ua.HasOne(e => e.User).WithMany(e => e.Assets).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}