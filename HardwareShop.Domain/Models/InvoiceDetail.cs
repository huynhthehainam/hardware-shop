using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Domain.Models
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
        // Calculate total cost
        public double GetTotalCost()
        {
            Unit? cashUnit = Invoice?.Shop?.CashUnit;
            if (cashUnit == null)
            {
                return 0;
            }

            double cost = Quantity * Price;
            return cashUnit.RoundValue(cost);
        }
    }
}
