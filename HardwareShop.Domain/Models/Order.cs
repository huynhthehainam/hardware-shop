using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Events;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public class Order : AuditableEntityBase
    {
        public Order() : base()
        {
        }

        public Order(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public Guid Id { get; set; } = Guid.CreateVersion7();

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

        public Guid AddOrderDetail(OrderDetail orderDetail)
        {
            orderDetail.OrderId = this.Id;
            this.Details?.Add(orderDetail);
            return orderDetail.Id;
        }
        public static Order CreateNew(Guid customerId, Guid shopId, Guid createdBy)
        {

            var order = new Order
            {
                Id = Guid.CreateVersion7(),
                CustomerId = customerId,
                ShopId = shopId,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy,
                Details = new List<OrderDetail>()
            };
            order.AddDomainEvent(new OrderCreatedEvent()
            {
                CreatedAt = order.CreatedDate,
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                ShopId = order.ShopId
            });
            return order;
        }

    }
}