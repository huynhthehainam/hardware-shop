using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ProductAssetConfiguration : ModelConfigurationBase<ProductAsset>
    {
        public ProductAssetConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = p =>
            {
                _ = p.HasQueryFilter(e => e.Product != null && !e.Product.IsDeleted);
                _ = p.HasKey(p => p.Id);
                _ = p.HasOne(e => e.Product).WithMany(e => e.ProductAssets).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}