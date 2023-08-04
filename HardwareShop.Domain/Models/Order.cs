

using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{
    public class Order : EntityBase, ITrackingDate
    {
        public Order()
        {
        }

        public Order(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

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
        private ICollection<OrderDetail>? details;
        public ICollection<OrderDetail>? Details
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref details) : details;
            set => details = value;
        }
        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoices) : invoices;
            set => invoices = value;
        }
    }
}