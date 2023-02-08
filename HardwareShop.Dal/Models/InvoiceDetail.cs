using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public sealed class InvoiceDetail : EntityBase
    {
        public InvoiceDetail()
        {
        }

        public InvoiceDetail(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public long Id { get; set; }
        public int InvoiceId { get; set; }
        private Invoice? invoice;
        public Invoice? Invoice
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoice) : invoice;
            set => invoice = value;
        }

        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref product) : product;
            set => product = value;
        }

        public double Quantity { get; set; }
        public string? Description { get; set; }

        public double Price
        {
            get; set;
        }
        public double OriginalPrice { get; set; }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceDetail>(m =>
            {
                m.HasKey(e => e.Id);
                m.HasOne(e => e.Invoice).WithMany(e => e.Details).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
                m.HasOne(e => e.Product).WithMany(e => e.InvoiceDetails).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
