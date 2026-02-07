
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Domain.Events
{
    public class OrderCreatedEvent : DomainEvent
    {
        public required Guid OrderId { get; set; }
        public required Guid CustomerId { get; set; }
        public required DateTime CreatedAt { get; set; } 
        public required Guid ShopId { get; set; }


    }
}