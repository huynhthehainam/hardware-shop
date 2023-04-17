using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ProductCategoryConfiguration : ModelConfigurationBase<ProductCategory>
    {
        public ProductCategoryConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                _ = e.HasKey(e => e.Id);
                _ = e.HasOne(e => e.Shop).WithMany(e => e.ProductCategories).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}