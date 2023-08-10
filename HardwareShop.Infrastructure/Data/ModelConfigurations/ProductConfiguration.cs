using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
      

        public void Configure(EntityTypeBuilder<Product> e)
        {
            _ = e.HasQueryFilter(e => !e.IsDeleted);
            _ = e.HasKey(e => e.Id);
            _ = e.HasOne(e => e.Unit).WithMany(e => e.Products).HasForeignKey(e => e.UnitId).OnDelete(DeleteBehavior.Cascade);
            _ = e.HasOne(e => e.Shop).WithMany(e => e.Products).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}