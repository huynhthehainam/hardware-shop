using HardwareShop.Application.CQRS.OrderArea.Interfaces;
using HardwareShop.Application.CQRS.OrderArea.Queries;
using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using MediatR;

namespace HardwareShop.Application.CQRS.OrderArea.Handlers;

public class GetDetailedOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetDetailedOrderByIdQuery, ApplicationResponse<DetailedOrderDto>>
{
    public async Task<ApplicationResponse<DetailedOrderDto>> Handle(GetDetailedOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();
        var shop = await userRepository.GetShopByUserIdAsync(currentUserId, cancellationToken);
        if (shop is null)
        {
            return ApplicationResponse<DetailedOrderDto>.Failure(ApplicationError.CreateNotFoundError("Shop not found for the current user."));
        }

        var order = await orderRepository.GetDetailedByIdAndShopIdAsync(request.Id, shop.Id, cancellationToken);
        if (order is null)
        {
            return ApplicationResponse<DetailedOrderDto>.Failure(ApplicationError.CreateNotFoundError("Order not found for this shop."));
        }

        var totalAmount = order.GetTotalAmount();
        var dto = new DetailedOrderDto
        {
            Id = order.Id,
            ShopName = order.Shop?.Name,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name,
            CustomerPhone = order.Customer?.Phone,
            CustomerPhonePrefix = order.Customer?.PhoneCountry?.PhonePrefix,
            CustomerAddress = order.Customer?.Address,
            CreatedDate = order.CreatedDate,
            CustomerDebtBeforeOrder = order.CustomerDebtBeforeOrder,
            PaidAmount = order.PaidAmount,
            TotalAmount = totalAmount,
            DebtAfterOrder = order.CustomerDebtBeforeOrder + totalAmount - order.PaidAmount,
            Details = order.Details?.Select(d => new DetailedOrderItemDto
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductName = d.Product?.Name,
                UnitId = d.UnitId,
                UnitName = d.Unit?.Name,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                TotalAmount = d.Quantity * d.UnitPrice,
                Note = d.Note
            }).ToArray() ?? Array.Empty<DetailedOrderItemDto>()
        };

        return ApplicationResponse<DetailedOrderDto>.Success(dto);
    }
}
