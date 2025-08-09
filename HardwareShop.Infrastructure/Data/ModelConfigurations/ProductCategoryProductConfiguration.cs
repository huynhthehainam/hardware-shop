using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class ProductCategoryProductConfiguration : IEntityTypeConfiguration<ProductCategoryProduct>
    {
       

        public void Configure(EntityTypeBuilder<ProductCategoryProduct> e)
        {
            _ = e.HasQueryFilter(e => e.Product != null && !e.Product.IsDeleted);
            _ = e.HasKey(e => new { e.ProductId, e.ProductCategoryId });
            _ = e.HasOne(e => e.Product).WithMany(e => e.ProductCategoryProducts).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
            _ = e.HasOne(e => e.ProductCategory).WithMany(e => e.ProductCategoryProducts).HasForeignKey(e => e.ProductCategoryId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}