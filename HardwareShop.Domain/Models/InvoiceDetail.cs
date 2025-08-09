using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public sealed class InvoiceDetail : EntityBase
    {
        public InvoiceDetail()
        {
        }

        public InvoiceDetail(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public long Id { get; set; }
        public int InvoiceId { get; set; }
        private Invoice? invoice;
        public Invoice? Invoice
        {
            get => lazyLoader?.Load(this, ref invoice);
            set => invoice = value;
        }
        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader?.Load(this, ref product);
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
