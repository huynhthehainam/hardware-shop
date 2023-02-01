using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareShop.WebApi.Controllers
{
    public class WarehousesController : AuthorizedApiControllerBase
    {
        private readonly IShopService shopService;
        public WarehousesController(IShopService shopService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.shopService = shopService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseOfShopCommand command)
        {
            var warehouse = await shopService.CreateWarehouseOfCurrentUserShopAsync(command.Name ?? "", command.Address);
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
            var warehousePageData = await shopService.GetWarehousesOfCurrentUserShopAsync(pagingModel, search);
            if (warehousePageData == null) { return responseResultBuilder.Build(); }

            responseResultBuilder.SetPageData(warehousePageData);

            return responseResultBuilder.Build();
        }
    }
}
