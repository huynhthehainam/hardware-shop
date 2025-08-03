using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
       

        public void Configure(EntityTypeBuilder<Warehouse> w)
        {
            _ = w.HasKey(e => e.Id);
            _ = w.HasOne(e => e.Shop).WithMany(e => e.Warehouses).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}