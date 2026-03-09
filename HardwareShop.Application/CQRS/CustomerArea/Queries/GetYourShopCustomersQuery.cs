using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.CustomerArea.Queries;

public class GetYourShopCustomersQuery : IRequest<ApplicationResponse<PageData<CustomerDto>>>
{
    public required PagingModel PagingModel { get; set; }
    public string? Search { get; set; }
}
