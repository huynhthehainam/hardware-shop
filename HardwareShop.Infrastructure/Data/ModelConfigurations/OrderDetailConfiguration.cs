using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.Data.ModelConfigurations
{
    public sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {


        public void Configure(EntityTypeBuilder<OrderDetail> m)
        {
            _ = m.HasKey(e => e.Id);
            _ = m.HasOne(e => e.Order).WithMany(e => e.Details).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Restrict);
            _ = m.HasOne(e => e.ProductUnit).WithMany(e => e.OrderDetails).HasForeignKey(e => e.ProductUnitId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}