using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {


        public void Configure(EntityTypeBuilder<Invoice> e)
        {
            _ = e.HasKey(e => e.Id);
            _ = e.HasOne(e => e.Customer).WithMany(e => e.Invoices).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
            _ = e.HasOne(e => e.Shop).WithMany(e => e.Invoices).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            _ = e.HasOne(e => e.Order).WithMany(e => e.Invoices).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
            _ = e.HasOne(e => e.CurrentDebtHistory).WithMany(e => e.Invoices).HasForeignKey(e => e.CurrentDebtHistoryId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}