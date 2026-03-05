using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.CustomerArea.Queries;

public class GetCustomerByIdQuery : IRequest<ApplicationResponse<CustomerDto>>
{
    public required Guid Id { get; set; }
}