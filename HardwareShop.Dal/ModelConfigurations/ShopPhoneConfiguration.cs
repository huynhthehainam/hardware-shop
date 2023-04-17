using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ShopPhoneConfiguration : ModelConfigurationBase<ShopPhone>
    {
        public ShopPhoneConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = sp =>
            {
                _ = sp.HasKey(s => s.Id);
                _ = sp.HasOne(e => e.Shop).WithMany(e => e.Phones).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                _ = sp.HasOne(e => e.Country).WithMany(e => e.ShopPhones).HasForeignKey(e => e.CountryId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}