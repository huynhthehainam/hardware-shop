using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class WarehouseConfiguration : ModelConfigurationBase<Warehouse>
    {
        public WarehouseConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = w =>
            {
                _ = w.HasKey(e => e.Id);
                _ = w.HasOne(e => e.Shop).WithMany(e => e.Warehouses).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}