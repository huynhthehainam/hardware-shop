using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ShopsController : AuthorizedApiControllerBase
    {
        private readonly IShopService shopService;
        private readonly ICustomerService customerService;
        private readonly IUserService userService;
        public ShopsController(ICustomerService customerService, IUserService userService, IShopService shopService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.customerService = customerService;
            this.userService = userService;
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
            if (command.Logo == null)
            {
                return responseResultBuilder.Build();
            }

            var shopAsset = await shopService.UpdateLogoAsync(id, command.Logo);
            if (shopAsset == null) return responseResultBuilder.Build();

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }

        [HttpPost("YourShop/UpdateLogo")]
        public async Task<IActionResult> UpdateYourShopLogo([FromForm] UpdateShopLogoCommand command)
        {
            if (command.Logo == null)
            {
                return responseResultBuilder.Build();
            }
            var shopAsset = await shopService.UpdateYourShopLogoAsync(command.Logo);
            if (shopAsset == null) return responseResultBuilder.Build();

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }


        [HttpGet("YourShop/Users")]
        public async Task<IActionResult> GetUsersOfYourShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            PageData<UserDto>? users = await userService.GetUserPageDataOfShopAsync(pagingModel, search);
            if (users == null) return responseResultBuilder.Build();

            responseResultBuilder.SetPageData(users);
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
