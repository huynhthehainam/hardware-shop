using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class CustomerDebtConfiguration : IEntityTypeConfiguration<CustomerDebt>
    {


        public void Configure(EntityTypeBuilder<CustomerDebt> e)
        {
            e.HasKey(e => e.CustomerId);
            e.HasOne(e => e.Customer).WithOne(e => e.Debt).HasForeignKey<CustomerDebt>(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}