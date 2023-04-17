using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ProductConfiguration : ModelConfigurationBase<Product>
    {
        public ProductConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                _ = e.HasQueryFilter(e => !e.IsDeleted);
                _ = e.HasKey(e => e.Id);
                _ = e.HasOne(e => e.Unit).WithMany(e => e.Products).HasForeignKey(e => e.UnitId).OnDelete(DeleteBehavior.Cascade);
                _ = e.HasOne(e => e.Shop).WithMany(e => e.Products).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}