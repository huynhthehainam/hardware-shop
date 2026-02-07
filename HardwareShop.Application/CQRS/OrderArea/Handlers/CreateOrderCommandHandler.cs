using HardwareShop.Application.CQRS.OrderArea.Commands;
using HardwareShop.Application.CQRS.OrderArea.Interfaces;
using HardwareShop.Application.CQRS.ProductArea.Interfaces;
using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Application.Extensions;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.OrderArea.Handlers;

public class CreateOrderCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService,
IMediator mediator,
IProductRepository productRepository, IOrderRepository orderRepository)
: IRequestHandler<CreateOrderCommand, ApplicationResponse<Guid>>
{
    public async Task<ApplicationResponse<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();
        var shop = await userRepository.GetShopByUserIdAsync(currentUserId, cancellationToken);
        if (shop == null)
        {
            return ApplicationResponse<Guid>.Failure(ApplicationError.CreateNotFoundError("Shop not found for the current user."));
        }
        var order = Order.CreateNew(request.CustomerId, shop.Id, currentUserId);
        var productIds = request.ProductItems.Select(pi => pi.ProductId).ToList();
        var products = await productRepository.GetProductsByProductIdsAsync(productIds, shop.Id, cancellationToken);
        if (products.Count != productIds.Count)
        {
            return ApplicationResponse<Guid>.Failure(ApplicationError.CreateInvalidError("One or more products are invalid for this shop."));
        }
        foreach (var item in request.ProductItems)
        {
            order.AddOrderDetail(new OrderDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Note = item.Note,
                UnitPrice = item.UnitPrice,
                UnitId = item.UnitId,

            });
        }
        order = await orderRepository.AddAsync(order, cancellationToken);
        foreach (var evt in order.GetDomainEvents())
        {
            await mediator.PublishDomainEventsAsync(evt, cancellationToken);
        }
        return ApplicationResponse<Guid>.Success(order.Id);
    }
}