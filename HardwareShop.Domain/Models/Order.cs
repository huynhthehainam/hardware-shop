using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

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

        public Guid Id { get; set; } = Guid.CreateVersion7();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public Guid CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader?.Load(this, ref customer);
            set => customer = value;
        }
        public Guid ShopId { get; set; }
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

    }
}