using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Application.CQRS.CustomerArea.Queries;
using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using MediatR;

namespace HardwareShop.Application.CQRS.CustomerArea.Handlers;

public class GetYourShopCustomersQueryHandler(
    ICustomerRepository customerRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetYourShopCustomersQuery, ApplicationResponse<PageData<CustomerDto>>>
{
    public async Task<ApplicationResponse<PageData<CustomerDto>>> Handle(GetYourShopCustomersQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();
        var shop = await userRepository.GetShopByUserIdAsync(currentUserId, cancellationToken);
        if (shop is null)
        {
            return ApplicationResponse<PageData<CustomerDto>>.Failure(ApplicationError.CreateNotFoundError("Shop not found for the current user."));
        }

        var customers = await customerRepository.GetPageDataByShopIdAsync(shop.Id, request.PagingModel, request.Search, cancellationToken);
        var mapped = customers.ConvertToOtherPageData(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Phone = c.Phone,
            Address = c.Address,
            PhonePrefix = c.PhoneCountry?.PhonePrefix,
            PhoneCountryId = c.PhoneCountryId,
            IsFamiliar = c.IsFamiliar,
            Debt = c.GetCurrentDebt()
        });

        return ApplicationResponse<PageData<CustomerDto>>.Success(mapped);
    }
}
