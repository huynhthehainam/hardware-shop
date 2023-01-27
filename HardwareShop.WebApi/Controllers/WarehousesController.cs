using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class WarehousesController : AuthorizedApiControllerBase
    {
        private readonly IShopService shopService;
        public WarehousesController(IShopService shopService, IResponseResultBuilder responseResultBuilder, ICurrentAccountService currentAccountService) : base(responseResultBuilder, currentAccountService)
        {
            this.shopService = shopService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseOfShopCommand command)
        {
            var warehouse = await shopService.CreateWarehouseOfCurrentAccountShopAsync(command.Name ?? "", command.Address);
            if (warehouse == null)
            {
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetData(warehouse);
            return responseResultBuilder.Build();
        }
    }
}
