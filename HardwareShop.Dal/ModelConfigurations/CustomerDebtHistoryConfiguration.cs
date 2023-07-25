using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class CustomerDebtHistoryConfiguration : IEntityTypeConfiguration<CustomerDebtHistory>
    {

        public void Configure(EntityTypeBuilder<CustomerDebtHistory> h)
        {
            h.HasKey(e => e.Id);
            h.HasOne(e => e.CustomerDebt).WithMany(e => e.Histories).HasForeignKey(e => e.CustomerDebtId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}