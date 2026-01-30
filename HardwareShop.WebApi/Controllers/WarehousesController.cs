using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class WarehousesController : AuthorizedApiControllerBase
    {
        public WarehousesController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseOfShopCommand command)
        {
            return responseResultBuilder.Build();
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouses([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Delete")]
        public async Task<IActionResult> DeleteWarehouse([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateQuantityForProduct")]
        public async Task<IActionResult> UpdateQuantityForProduct([FromRoute] int id, [FromBody] UpdateQuantityForProductCommand command)
        {
            return responseResultBuilder.Build();
        }
    }
}
