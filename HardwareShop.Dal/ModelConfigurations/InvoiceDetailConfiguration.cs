

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class InvoiceDetailConfiguration : ModelConfigurationBase<InvoiceDetail>
    {
        public InvoiceDetailConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = m =>
            {
                _ = m.HasKey(e => e.Id);
                _ = m.HasOne(e => e.Invoice).WithMany(e => e.Details).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
                _ = m.HasOne(e => e.Product).WithMany(e => e.InvoiceDetails).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}