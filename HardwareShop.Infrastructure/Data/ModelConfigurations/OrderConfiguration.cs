using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {

        public void Configure(EntityTypeBuilder<Order> e)
        {
            _ = e.HasKey(e => e.Id);
            _ = e.HasOne(e => e.Customer).WithMany(e => e.Orders).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
            _ = e.HasOne(e => e.Shop).WithMany(e => e.Orders).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}