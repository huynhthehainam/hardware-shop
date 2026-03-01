
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
        var productIds = orderEvent.Order.Details?.Select(d => d.ProductId).ToList() ?? [];
        var warehouseProducts = await warehouseRepository.GetWarehouseProductsByProductUnitIdsAsync(productIds, cancellationToken);
        foreach (var detail in orderEvent.Order.Details ?? [])
        {
            var totalRequiredQuantity = detail.Quantity;
            var selectedWarehouseProduct = warehouseProducts.Where(wp => wp.ProductId == detail.ProductId && wp.Product!.UnitId == detail.UnitId).OrderBy(wp => wp.Quantity);
            foreach (var warehouseProduct in selectedWarehouseProduct)
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
                logger.LogError(
                    "Insufficient stock for Product {ProductId} with Unit {UnitId} to fulfill Order {OrderId}",
                    detail.ProductId,
                    detail.UnitId,
                    orderEvent.Order.Id);
                throw new InvalidOperationException($"Insufficient stock for Product {detail.ProductId} with Unit {detail.UnitId}");
            }

        }

    }
}
