using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class OrderDetailConfiguration : ModelConfigurationBase<OrderDetail>
    {
        public OrderDetailConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = m =>
            {
                _ = m.HasQueryFilter(e => e.Product != null && !e.Product.IsDeleted);
                _ = m.HasKey(e => e.Id);
                _ = m.HasOne(e => e.Order).WithMany(e => e.Details).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
                _ = m.HasOne(e => e.Product).WithMany(e => e.OrderDetails).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}