using System.Globalization;
using HardwareShop.Application.CQRS.WarehouseArea.Interfaces;
using HardwareShop.Application.Localization;
using HardwareShop.Domain.Events;
using HardwareShop.Domain.Models;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HardwareShop.Application.CQRS.WarehouseArea.Handlers
{
    public class ShopCreatedHandler(IWarehouseRepository warehouseRepository, IStringLocalizer<WarehouseResources> localizer, ILogger<ShopCreatedHandler> logger) : INotificationHandler<DomainEventNotification<ShopCreatedEvent>>
    {
        public async Task Handle(DomainEventNotification<ShopCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var warehouse = Warehouse.Create(
              name: localizer["MainWarehouse", notification.DomainEvent.Name],
              address: null,
              shopId: notification.DomainEvent.ShopId);
            await warehouseRepository.AddAsync(
                  warehouse,
                    cancellationToken);

        }
    }
}