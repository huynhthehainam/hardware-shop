

using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class OrdersController : AuthorizedApiControllerBase
    {
        private readonly IOrderService orderService;
        public OrdersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, IOrderService orderService) : base(responseResultBuilder, currentUserService)
        {
            this.orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var order = await orderService.CreateOrder();
            return responseResultBuilder.Build();
        }
    }
}