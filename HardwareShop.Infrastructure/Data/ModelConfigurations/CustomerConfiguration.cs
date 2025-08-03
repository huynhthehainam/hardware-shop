using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {

        public void Configure(EntityTypeBuilder<Customer> e)
        {
            e.HasKey(e => e.Id);
            e.HasOne(e => e.Shop).WithMany(e => e.Customers).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(e => e.PhoneCountry).WithMany(e => e.Customers).HasForeignKey(e => e.PhoneCountryId).OnDelete(DeleteBehavior.SetNull);

        }
    }
}