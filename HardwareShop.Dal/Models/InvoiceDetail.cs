using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public double Profit { get; set; }
        public double TotalCost { get; set; }
    }
}
