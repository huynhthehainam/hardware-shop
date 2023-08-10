using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ShopsController : AuthorizedApiControllerBase
    {
        private readonly IShopService shopService;

        private readonly IUserService userService;
        public ShopsController(IUserService userService, IShopService shopService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.userService = userService;
            this.shopService = shopService;
        }
        [HttpGet]
        public async Task<IActionResult> GetShops([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            if (!currentUserService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }

            PageData<ShopItemDto> shopPageData = await shopService.GetShopDtoPageDataAsync(pagingModel, search);
            responseResultBuilder.SetPageData(shopPageData);
            return responseResultBuilder.Build();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
        {
            if (!currentUserService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }
            var response = await shopService.CreateShopAsync(command.Name ?? "", command.Address, command.CashUnitId.GetValueOrDefault());
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
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

            var response = await shopService.UpdateLogoAsync(id, command.Logo);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
            return responseResultBuilder.Build();
        }

        [HttpPost("YourShop/UpdateLogo")]
        public async Task<IActionResult> UpdateYourShopLogo([FromForm] UpdateShopLogoCommand command)
        {
            if (command.Logo == null)
            {
                return responseResultBuilder.Build();
            }
            var response = await shopService.UpdateYourShopLogoAsync(command.Logo);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateSetting")]
        public async Task<IActionResult> UpdateShopSettings([FromRoute] int id, [FromBody] UpdateShopSettingCommand command)
        {
            var response = await shopService.UpdateShopSettingAsync(id, command.IsAllowedToShowInvoiceDownloadOptions);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
            return responseResultBuilder.Build();
        }

        [HttpGet("YourShop/Logo")]
        public async Task<IActionResult> GetYourShopLogo()
        {
            var response = await shopService.GetCurrentUserShopLogoAsync();
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetAsset(result));
            return responseResultBuilder.Build();
        }


        [HttpGet("YourShop/Users")]
        public async Task<IActionResult> GetUsersOfYourShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var response = await userService.GetUserPageDataOfShopAsync(pagingModel, search);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetPageData(result));
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
            var response = await shopService.DeleteShopSoftlyAsync(id);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
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
            var response = await shopService.CreateAdminUserAsync(id, command.Username ?? "", command.Password ?? "", command.Email);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
            return responseResultBuilder.Build();
        }

    }
}
