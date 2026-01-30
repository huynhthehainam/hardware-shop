
using System.Data;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Events;
using MediatR;

namespace HardwareShop.Application.Extensions
{
    public static class MediatorExtensions
    {
        public static async Task PublishDomainEventsAsync(this IMediator mediator, DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            await mediator.Publish(MappingType(domainEvent), cancellationToken);
        }
        public static INotification MappingType(DomainEvent domainEvent)
        => domainEvent switch
        {
            ShopCreatedEvent shopCreatedEvent => new DomainEventNotification<ShopCreatedEvent>(shopCreatedEvent),
            _ => throw new ArgumentException($"No mapping for domain event type {domainEvent.GetType()}"),
        };
    }
}