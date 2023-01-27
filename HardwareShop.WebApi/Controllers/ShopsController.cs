using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ShopsController : AuthorizedApiController
    {
        private readonly IShopService shopService;
        public ShopsController(IShopService shopService, IResponseResultBuilder responseResultBuilder, ICurrentAccountService currentAccountService) : base(responseResultBuilder, currentAccountService)
        {
            this.shopService = shopService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
        {

            var shop = await shopService.CreateShopAsync(command.Name ?? "", command.Address);

            responseResultBuilder.SetData(shop);
            return responseResultBuilder.Build();    
        }
        
    }
}
