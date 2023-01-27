using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class Invoice : EntityBase
    {
        public Invoice()
        {
        }

        public Invoice(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customer) : customer;
            set => customer = value;
        }

        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }
        private ICollection<InvoiceDetail>? details;
        public ICollection<InvoiceDetail>? Details
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref details) : details;
            set => details = value;
        }

        public double ChangeOfDebt { get; set; }

        public double CurrentDebt { get; set; }

        public double Deposit { get; set; }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(e =>
                {
                    e.HasKey(e => e.Id);
                    e.HasOne(e => e.Customer).WithMany(e => e.Invoices).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(e => e.Shop).WithMany(e => e.Invoices).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
