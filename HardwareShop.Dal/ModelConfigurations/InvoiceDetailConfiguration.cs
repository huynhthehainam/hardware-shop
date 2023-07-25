using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
    {


        public void Configure(EntityTypeBuilder<InvoiceDetail> m)
        {
            _ = m.HasQueryFilter(e => e.Product != null && !e.Product.IsDeleted);
            _ = m.HasKey(e => e.Id);
            _ = m.HasOne(e => e.Invoice).WithMany(e => e.Details).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            _ = m.HasOne(e => e.Product).WithMany(e => e.InvoiceDetails).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}