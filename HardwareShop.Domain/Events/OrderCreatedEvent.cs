
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Domain.Events
{
    public class OrderCreatedEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public Guid UserId { get; }
        public DateTime CreatedAt { get; }

        public OrderCreatedEvent(Guid orderId, Guid userId, DateTime createdAt)
        {
            OrderId = orderId;
            UserId = userId;
            CreatedAt = createdAt;
        }
    }
}