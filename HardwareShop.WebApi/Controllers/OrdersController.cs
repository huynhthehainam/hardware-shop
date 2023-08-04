using HardwareShop.Application.Services;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class OrdersController : AuthorizedApiControllerBase
    {

        public OrdersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {

        }
        [HttpPost]
        public Task<IActionResult> CreateOrder()
        {

            return Task.FromResult(responseResultBuilder.Build());
        }
    }
}