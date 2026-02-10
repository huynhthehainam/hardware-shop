using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Models;

namespace HardwareShop.Domain.Events;

public class CustomerCreatedEvent : DomainEvent
{
    public required Customer Customer { get; set; }
}