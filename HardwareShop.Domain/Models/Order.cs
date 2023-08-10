

using HardwareShop.Core.Bases;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public class Order : EntityBase, ITrackingDate
    {
        public Order()
        {
        }

        public Order(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader?.Load(this, ref customer);
            set => customer = value;
        }
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }
        private ICollection<OrderDetail>? details;
        public ICollection<OrderDetail>? Details
        {
            get => lazyLoader?.Load(this, ref details);
            set => details = value;
        }
        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader?.Load(this, ref invoices);
            set => invoices = value;
        }
    }
}