using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ShopConfiguration : IEntityTypeConfiguration<Shop>
    {
       

        public void Configure(EntityTypeBuilder<Shop> s)
        {
            _ = s.HasKey(x => x.Id);
            _ = s.HasOne(e => e.CashUnit).WithMany(e => e.Shops).HasForeignKey(e => e.CashUnitId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}