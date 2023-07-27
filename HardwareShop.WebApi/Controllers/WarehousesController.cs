using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
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
            var warehousePageData = await warehouseService.GetWarehousesOfCurrentUserShopAsync(pagingModel, search);
            if (warehousePageData == null) { return responseResultBuilder.Build(); }
            responseResultBuilder.SetPageData(warehousePageData);
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Delete")]
        public async Task<IActionResult> DeleteWarehouse([FromRoute] int id)
        {
            var isSuccess = await warehouseService.DeleteWarehouseOfCurrentUserShopAsync(id);
            if (!isSuccess)
            {
                responseResultBuilder.AddNotFoundEntityError("Warehouse");
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetDeletedMessage();
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateQuantityForProduct")]
        public async Task<IActionResult> UpdateQuantityForProduct([FromRoute] int id, [FromBody] UpdateQuantityForProductCommand command)
        {
            WarehouseProductDto? warehouseProduct = await warehouseService.CreateOrUpdateWarehouseProductAsync(id, command.ProductId.GetValueOrDefault(), command.Quantity.GetValueOrDefault());
            if (warehouseProduct == null)
            {
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetData(warehouseProduct);
            return responseResultBuilder.Build();
        }
    }
}
