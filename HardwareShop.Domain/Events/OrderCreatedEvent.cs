
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Models;

namespace HardwareShop.Domain.Events
{
    public class OrderCreatedEvent : DomainEvent
    {
        public required Order Order { get; set; }
    }
}