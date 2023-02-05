using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ShopsController : AuthorizedApiControllerBase
    {
        private readonly IShopService shopService;
        public ShopsController(IShopService shopService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.shopService = shopService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
        {
            if (!currentUserService.IsSystemAdmin())
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
        [HttpPost("{id:int}/UpdateLogo")]
        public async Task<IActionResult> UpdateLogo([FromRoute] int id, [FromForm] UpdateShopLogoCommand command)
        {
            if (!currentUserService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();

        }



        [HttpPost("{id:int}/DeleteSoftly")]
        public async Task<IActionResult> DeleteShopSoftly([FromRoute] int id)
        {
            if (!currentUserService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }
            var isSuccess = await shopService.DeleteShopSoftlyAsync(id);
            if (!isSuccess) return responseResultBuilder.Build();
            responseResultBuilder.SetDeletedMessage();
            return responseResultBuilder.Build();
        }

        [HttpPost("{id:int}/CreateAdminUser")]
        public async Task<IActionResult> CreateAdminUser([FromRoute] int id, CreateShopAdminUserCommand command)
        {
            if (!currentUserService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }

            var user = await shopService.CreateAdminUserAsync(id, command.Username ?? "", command.Password ?? "", command.Email);

            responseResultBuilder.SetData(user);
            return responseResultBuilder.Build();
        }

    }
}
