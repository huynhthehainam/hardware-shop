using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ProductCategoryProductConfiguration : ModelConfigurationBase<ProductCategoryProduct>
    {
        public ProductCategoryProductConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                _ = e.HasKey(e => new { e.ProductId, e.ProductCategoryId });
                _ = e.HasOne(e => e.Product).WithMany(e => e.ProductCategoryProducts).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                _ = e.HasOne(e => e.ProductCategory).WithMany(e => e.ProductCategoryProducts).HasForeignKey(e => e.ProductCategoryId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}