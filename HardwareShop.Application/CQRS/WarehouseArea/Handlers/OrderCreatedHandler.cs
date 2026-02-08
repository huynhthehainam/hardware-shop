
using HardwareShop.Application.CQRS.WarehouseArea.Interfaces;
using HardwareShop.Domain.Events;
using HardwareShop.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HardwareShop.Application.CQRS.WarehouseArea.Handlers;

public class OrderCreatedHandler(IWarehouseRepository warehouseRepository, IRepository<WarehouseProduct> warehouseProductRepository, ILogger<OrderCreatedHandler> logger) : INotificationHandler<DomainEventNotification<OrderCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<OrderCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var orderEvent = notification.DomainEvent;
        var productIds = orderEvent.Order.Details?.Select(d => d.ProductUnit!.ProductId).ToList() ?? [];
        var warehouseProducts = await warehouseRepository.GetwarehouseProductsByProductUnitIdsAsync(productIds, cancellationToken);
        foreach (var detail in orderEvent.Order.Details ?? [])
        {
            var totalRequiredQuantity = detail.Quantity;
            var sameUnitWarehouseProductsForDetail = warehouseProducts.Where(wp => wp.ProductUnitId == detail.ProductUnitId).OrderBy(wp => wp.Quantity);
            foreach (var warehouseProduct in sameUnitWarehouseProductsForDetail)
            {
                if (totalRequiredQuantity <= 0)
                {
                    break;
                }
                warehouseProduct.Quantity -= totalRequiredQuantity;
                totalRequiredQuantity = 0;
                await warehouseProductRepository.UpdateAsync(warehouseProduct, cancellationToken);
                await warehouseProductRepository.SaveChangesAsync(cancellationToken);

            }
            if (totalRequiredQuantity > 0)
            {
                logger.LogError("Insufficient stock for ProductUnit {ProductUnitId} to fulfill Order {OrderId}", detail.ProductUnitId, orderEvent.Order.Id);
                throw new InvalidOperationException($"Insufficient stock for ProductUnit {detail.ProductUnitId}");
            }

        }

    }
}