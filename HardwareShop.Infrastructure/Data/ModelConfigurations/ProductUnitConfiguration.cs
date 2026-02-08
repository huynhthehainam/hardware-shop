using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.Data.ModelConfigurations;

public class ProductUnitConfiguration : IEntityTypeConfiguration<ProductUnit>
{
    public void Configure(EntityTypeBuilder<ProductUnit> pu)
    {
        _ = pu.HasKey(e => e.Id);
        _ = pu.HasOne(e => e.Product).WithMany(e => e.ProductUnits).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
        _ = pu.HasOne(e => e.Unit).WithMany(e => e.ProductUnits).HasForeignKey(e => e.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}