using HardwareShop.Application.CQRS.ShopArea.Commands;
using HardwareShop.Application.CQRS.ShopArea.Interfaces;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Extensions;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Events;
using HardwareShop.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HardwareShop.Application.CQRS.ShopArea.Handlers
{
    public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, ApplicationResponse<CreatedShopDto>>
    {
        private readonly ILogger<CreateShopCommandHandler> logger;
        private readonly IShopRepository shopRepository;
        private readonly IMediator mediator;
        public CreateShopCommandHandler(ILogger<CreateShopCommandHandler> logger, IShopRepository shopRepository, IMediator mediator)
        {
            this.logger = logger;
            this.shopRepository = shopRepository;
            this.mediator = mediator;
        }
        public async Task<ApplicationResponse<CreatedShopDto>> Handle(CreateShopCommand request, CancellationToken cancellationToken)
        {
            var shop = Shop.CreateShop(request.Name, request.CashUnitId, request.Address);
            shop = await shopRepository.AddAsync(shop, cancellationToken);
            foreach (var evt in shop.GetDomainEvents())
            {
                await mediator.PublishDomainEventsAsync(evt, cancellationToken);
            }
            return new ApplicationResponse<CreatedShopDto>(new CreatedShopDto
            {
                Id = shop.Id,

            });
        }
    }
}