using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Application.Extensions;
using HardwareShop.Domain.Events;
using MediatR;

namespace HardwareShop.Application.CQRS.CustomerArea.Handlers;

public class OrderCreatedHandler(ICustomerRepository customerRepository, IMediator mediator) : INotificationHandler<DomainEventNotification<OrderCreatedEvent>>
{
    // Handler implementation
    public async Task Handle(DomainEventNotification<OrderCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var order = notification.DomainEvent.Order;
        var customer = order.Customer;
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer not found for Order {order.Id}");
        }
        customer.AddDebt(order.GetTotalAmount() - order.PaidAmount);
        foreach (var domainEvent in customer.GetDomainEvents())
        {
            await mediator.PublishDomainEventsAsync(domainEvent, cancellationToken);
        }
        await customerRepository.UpdateAsync(customer, cancellationToken);
        await customerRepository.SaveChangesAsync(cancellationToken);
    }
}