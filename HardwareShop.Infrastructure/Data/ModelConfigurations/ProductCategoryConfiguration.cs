using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
    
        public void Configure(EntityTypeBuilder<ProductCategory> e)
        {
            _ = e.HasKey(e => e.Id);
            _ = e.HasOne(e => e.Shop).WithMany(e => e.ProductCategories).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}