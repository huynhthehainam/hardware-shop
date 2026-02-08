using HardwareShop.Application.CQRS.OrderArea.Commands;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class OrdersController : AuthorizedApiControllerBase
    {
        private readonly IMediator mediator;
        public OrdersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, IMediator mediator) : base(responseResultBuilder, currentUserService)
        {
            this.mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
        {
            var response = await mediator.Send(command);
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }
    }
}