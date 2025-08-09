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
        private readonly IWarehouseService warehouseService;
        public WarehousesController(IWarehouseService warehouseService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.warehouseService = warehouseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseOfShopCommand command)
        {
            var warehouse = await warehouseService.CreateWarehouseOfCurrentUserShopAsync(command.Name ?? "", command.Address);
            if (warehouse == null)
            {
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetData(warehouse);
            return responseResultBuilder.Build();
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouses([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var response = await warehouseService.GetWarehousesOfCurrentUserShopAsync(pagingModel, search);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetPageData(result));
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Delete")]
        public async Task<IActionResult> DeleteWarehouse([FromRoute] int id)
        {
            var response = await warehouseService.DeleteWarehouseOfCurrentUserShopAsync(id);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetDeletedMessage());
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateQuantityForProduct")]
        public async Task<IActionResult> UpdateQuantityForProduct([FromRoute] int id, [FromBody] UpdateQuantityForProductCommand command)
        {
            var response = await warehouseService.CreateOrUpdateWarehouseProductAsync(id, command.ProductId.GetValueOrDefault(), command.Quantity.GetValueOrDefault());
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
            return responseResultBuilder.Build();
        }
    }
}
