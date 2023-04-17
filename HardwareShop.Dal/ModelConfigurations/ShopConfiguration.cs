using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ShopConfiguration : ModelConfigurationBase<Shop>
    {
        public ShopConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = s =>
            {
                _ = s.HasKey(x => x.Id);
                _ = s.HasOne(e => e.CashUnit).WithMany(e => e.Shops).HasForeignKey(e => e.CashUnitId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}