using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class ShopPhoneConfiguration : IEntityTypeConfiguration<ShopPhone>
    {


        public void Configure(EntityTypeBuilder<ShopPhone> sp)
        {
            _ = sp.HasKey(s => s.Id);
            _ = sp.HasOne(e => e.Shop).WithMany(e => e.Phones).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Restrict);
            _ = sp.HasOne(e => e.Country).WithMany(e => e.ShopPhones).HasForeignKey(e => e.CountryId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}