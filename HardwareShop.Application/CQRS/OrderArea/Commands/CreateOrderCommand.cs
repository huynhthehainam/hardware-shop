
using HardwareShop.Application.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.OrderArea.Commands;

public sealed class ProductItem
{
    public required Guid ProductId { get; set; }
    public required int Quantity { get; set; }
    public required double UnitPrice { get; set; }
    public string? Note { get; set; }
    public required int UnitId { get; set; }
}
public sealed class CreateOrderCommand : IRequest<ApplicationResponse<Guid>>
{
    public required Guid UserId { get; set; }
    public required List<ProductItem> ProductItems { get; set; }
    public required Guid CustomerId { get; set; }

}