
using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class InvoiceConfiguration : ModelConfigurationBase<Invoice>
    {
        public InvoiceConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
                    {
                        _ = e.HasKey(e => e.Id);
                        _ = e.HasOne(e => e.Customer).WithMany(e => e.Invoices).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
                        _ = e.HasOne(e => e.Shop).WithMany(e => e.Invoices).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                        _ = e.HasOne(e => e.Order).WithMany(e => e.Invoices).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
                        _ = e.HasOne(e => e.CurrentDebtHistory).WithMany(e => e.Invoices).HasForeignKey(e => e.CurrentDebtHistoryId).OnDelete(DeleteBehavior.Cascade);
                    };
        }
    }
}