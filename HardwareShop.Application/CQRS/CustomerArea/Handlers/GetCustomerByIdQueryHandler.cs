using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Application.CQRS.CustomerArea.Queries;
using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using MediatR;

namespace HardwareShop.Application.CQRS.CustomerArea.Handlers;

public class GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, ICurrentUserService currentUserService, IUserRepository userRepository) : IRequestHandler<GetCustomerByIdQuery, ApplicationResponse<CustomerDto>>
{

    public async Task<ApplicationResponse<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();
        var shop = await userRepository.GetShopByUserIdAsync(currentUserId, cancellationToken);
        if (shop is null)
        {
            return ApplicationResponse<CustomerDto>.Failure(ApplicationError.CreateNotFoundError("Shop not found for the current user."));
        }
        var customer = await customerRepository.GetByIdAndShopIdAsync(request.Id, shop.Id, cancellationToken);
        if (customer is null)
        {
            return ApplicationResponse<CustomerDto>.Failure(ApplicationError.CreateNotFoundError("Customer not found for this shop."));
        }

        var dto = new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Phone = customer.Phone,
            Address = customer.Address,
            PhonePrefix = customer.PhoneCountry?.PhonePrefix,
            PhoneCountryId = customer.PhoneCountryId,
            IsFamiliar = customer.IsFamiliar,
            Debt = customer.GetCurrentDebt()
        };

        return ApplicationResponse<CustomerDto>.Success(dto);
    }
}
