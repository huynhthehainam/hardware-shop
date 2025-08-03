using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {


        public void Configure(EntityTypeBuilder<Invoice> e)
        {
            _ = e.HasKey(e => e.Id);
            _ = e.HasOne(e => e.Customer).WithMany(e => e.Invoices).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
            _ = e.HasOne(e => e.Shop).WithMany(e => e.Invoices).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Restrict);
            _ = e.HasOne(e => e.Order).WithMany(e => e.Invoices).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Restrict);
            _ = e.HasOne(e => e.CurrentDebtHistory).WithMany(e => e.Invoices).HasForeignKey(e => e.CurrentDebtHistoryId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}