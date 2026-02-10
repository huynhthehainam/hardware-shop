
using MediatR;
using HardwareShop.Application.CQRS.CustomerArea.Commands;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;
using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Application.Extensions;

namespace HardwareShop.Application.CQRS.CustomerArea.Handlers;

public class CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMediator mediator) : IRequestHandler<CreateCustomerCommand, ApplicationResponse<Guid>>
{
    public async Task<ApplicationResponse<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(request.Name, request.Phone, request.PhoneCountryId, request.Address, request.IsFamiliar);
        await customerRepository.AddAsync(customer, cancellationToken);
        await customerRepository.SaveChangesAsync();
        foreach (var evt in customer.GetDomainEvents())
        {
            await mediator.PublishDomainEventsAsync(evt);
        }
        return ApplicationResponse<Guid>.Success(customer.Id);
    }
}