using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ShopsController : AuthorizedApiControllerBase
    {
        private readonly IShopService shopService;
        public ShopsController(IShopService shopService, IResponseResultBuilder responseResultBuilder, ICurrentAccountService currentAccountService) : base(responseResultBuilder, currentAccountService)
        {
            this.shopService = shopService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
        {
            if (!currentAccountService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }
            var shop = await shopService.CreateShopAsync(command.Name ?? "", command.Address);
            if (shop == null)
            {
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetData(shop);
            return responseResultBuilder.Build();
        }



        [HttpPost("{id:int}/DeleteSoftly")]
        public async Task<IActionResult> DeleteShopSoftly([FromRoute] int id)
        {
            if (!currentAccountService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }
            var isSuccess = await shopService.DeleteShopSoftlyAsync(id);
            if (!isSuccess) return responseResultBuilder.Build();
            responseResultBuilder.SetDeletedMessage();
            return responseResultBuilder.Build();
        }

        [HttpPost("{id:int}/CreateAdminAccount")]
        public async Task<IActionResult> CreateAdminAccount([FromRoute] int id, CreateShopAdminAccountCommand command)
        {
            if (!currentAccountService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }

            var account = await shopService.CreateAdminAccountAsync(id, command.Username ?? "", command.Password ?? "", command.Email);

            responseResultBuilder.SetData(account);
            return responseResultBuilder.Build();
        }

    }
}
