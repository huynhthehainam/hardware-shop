using HardwareShop.Domain.Abstracts;
using MediatR;
namespace HardwareShop.Application;
public class DomainEventNotification<TDomainEvent>
    : INotification
    where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}