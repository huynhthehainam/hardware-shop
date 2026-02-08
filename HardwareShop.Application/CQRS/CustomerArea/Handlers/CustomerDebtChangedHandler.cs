using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Domain.Events;
using MediatR;

namespace HardwareShop.Application.CQRS.CustomerArea.Handlers;

public class CustomerDebtChangedHandler(ICustomerRepository customerRepository) : INotificationHandler<DomainEventNotification<CustomerDebtChangedEvent>>
{
    public async Task Handle(DomainEventNotification<CustomerDebtChangedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var customer = await customerRepository.GetByIdAsync(domainEvent.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer not found for Id {domainEvent.CustomerId}");
        }
        var debtHistory = new Domain.Models.CustomerDebtHistory
        {
            OldDebt = domainEvent.OldDebt,
            ChangeOfDebt = domainEvent.ChangeOfDebt,
            CustomerDebtId = customer.Debt!.CustomerId
        };
        customer.Debt!.Histories!.Add(debtHistory);
        await customerRepository.UpdateAsync(customer, cancellationToken);
        await customerRepository.SaveChangesAsync(cancellationToken);
    }
}