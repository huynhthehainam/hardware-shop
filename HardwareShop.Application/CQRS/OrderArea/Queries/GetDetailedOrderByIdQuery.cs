using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.OrderArea.Queries;

public class GetDetailedOrderByIdQuery : IRequest<ApplicationResponse<DetailedOrderDto>>
{
    public required Guid Id { get; set; }
}
