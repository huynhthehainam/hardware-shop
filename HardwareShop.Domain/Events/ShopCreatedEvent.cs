
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Enums;

namespace HardwareShop.Domain.Events;

public class ShopCreatedEvent : DomainEvent
{
    public Guid ShopId { get; set; }
    public required string Name { get; set; }
    public required Language Language { get; set; }
}