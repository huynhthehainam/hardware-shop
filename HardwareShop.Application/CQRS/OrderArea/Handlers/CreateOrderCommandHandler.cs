using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
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
IMediator mediator, ITransactionService transactionService,
ICustomerRepository customerRepository,
IProductRepository productRepository, IOrderRepository orderRepository)
: IRequestHandler<CreateOrderCommand, ApplicationResponse<Guid>>
{
    public async Task<ApplicationResponse<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        using var tx = await transactionService.BeginTransactionAsync(cancellationToken);
        try
        {
            var currentUserId = currentUserService.GetUserId();
            var shop = await userRepository.GetShopByUserIdAsync(currentUserId, cancellationToken);
            if (shop == null)
            {
                return ApplicationResponse<Guid>.Failure(ApplicationError.CreateNotFoundError("Shop not found for the current user."));
            }
            var customer = await customerRepository.GetByIdAndShopIdAsync(request.CustomerId, shop.Id, cancellationToken);
            if (customer == null)
            {
                return ApplicationResponse<Guid>.Failure(ApplicationError.CreateNotFoundError("Customer not found for this shop."));
            }
            var order = Order.CreateNew(customer, shop.Id, request.PaidAmount, currentUserId);
            var productIds = request.ProductItems.Select(pi => pi.ProductId).ToList();
            var products = await productRepository.GetProductsByProductIdsAsync(productIds, shop.Id, cancellationToken);
            if (products.Count != productIds.Count)
            {
                return ApplicationResponse<Guid>.Failure(ApplicationError.CreateInvalidError("One or more products are invalid for this shop."));
            }
            var productAndUnitIds = request.ProductItems.Select(pi => (pi.ProductId, pi.UnitId)).Select(t => (t.Item1, t.Item2)).ToList();
            foreach (var item in request.ProductItems)
            {

                order.AddOrderDetail(new OrderDetail
                {
                    Quantity = item.Quantity,
                    Note = item.Note,
                    UnitPrice = item.UnitPrice,
                    ProductId = item.ProductId,
                    UnitId = item.UnitId
                });
            }
            order = await orderRepository.AddAsync(order, cancellationToken);
            await orderRepository.SaveChangesAsync(cancellationToken);
            foreach (var evt in order.GetDomainEvents())
            {
                await mediator.PublishDomainEventsAsync(evt, cancellationToken);
            }
            tx.Commit();
            return ApplicationResponse<Guid>.Success(order.Id);
        }
        catch (Exception ex)
        {
            tx.Rollback();
            return ApplicationResponse<Guid>.Failure(ApplicationError.CreateExceptionError("An error occurred while creating the order.", ex));
        }
    }
}