

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class OrderConfiguration : ModelConfigurationBase<Order>
    {
        public OrderConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
                {
                    _ = e.HasKey(e => e.Id);
                    _ = e.HasOne(e => e.Customer).WithMany(e => e.Orders).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
                    _ = e.HasOne(e => e.Shop).WithMany(e => e.Orders).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                };
        }
    }
}