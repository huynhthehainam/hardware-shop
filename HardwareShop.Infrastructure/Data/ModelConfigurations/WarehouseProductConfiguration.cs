using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class WarehouseProductConfiguration : IEntityTypeConfiguration<WarehouseProduct>
    {


        public void Configure(EntityTypeBuilder<WarehouseProduct> e)
        {
            _ = e.HasQueryFilter(e => e.Product != null && !e.Product.IsDeleted);
            _ = e.HasKey(e => new { e.ProductId, e.WarehouseId });
            _ = e.HasOne(e => e.Product).WithMany(e => e.WarehouseProducts).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            _ = e.HasOne(e => e.Warehouse).WithMany(e => e.WarehouseProducts).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}