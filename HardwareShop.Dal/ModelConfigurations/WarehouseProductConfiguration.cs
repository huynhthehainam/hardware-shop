using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class WarehouseProductConfiguration : ModelConfigurationBase<WarehouseProduct>
    {
        public WarehouseProductConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                _ = e.HasQueryFilter(e => e.Product != null && !e.Product.IsDeleted);
                _ = e.HasKey(e => new { e.ProductId, e.WarehouseId });
                _ = e.HasOne(e => e.Product).WithMany(e => e.WarehouseProducts).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                _ = e.HasOne(e => e.Warehouse).WithMany(e => e.WarehouseProducts).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}