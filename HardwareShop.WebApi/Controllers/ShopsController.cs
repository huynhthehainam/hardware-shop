using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Extensions;

using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Commands;
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
            CreatedShopDto? shop = await shopService.CreateShopAsync(command.Name ?? "", command.Address);
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

            ShopAssetDto? shopAsset = await shopService.UpdateLogoAsync(id, command.Logo);
            if (shopAsset == null)
            {
                return responseResultBuilder.Build();
            }

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
            ShopAssetDto? shopAsset = await shopService.UpdateYourShopLogoAsync(command.Logo);
            if (shopAsset == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateSetting")]
        public async Task<IActionResult> UpdateShopSettings([FromRoute] int id, [FromBody] UpdateShopSettingCommand command)
        {
            var isSuccess = await shopService.UpdateShopSettingAsync(id, command.IsAllowedToShowInvoiceDownloadOptions);
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }

        [HttpGet("YourShop/Logo")]
        public async Task<IActionResult> GetYourShopLogo()
        {
            CachedAsset? asset = await shopService.GetCurrentUserShopLogo();
            if (asset == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }


        [HttpGet("YourShop/Users")]
        public async Task<IActionResult> GetUsersOfYourShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            PageData<UserDto>? users = await userService.GetUserPageDataOfShopAsync(pagingModel, search);
            if (users == null)
            {
                return responseResultBuilder.Build();
            }

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
            bool isSuccess = await shopService.DeleteShopSoftlyAsync(id);
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

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
            CreatedUserDto? user = await shopService.CreateAdminUserAsync(id, command.Username ?? "", command.Password ?? "", command.Email);
            responseResultBuilder.SetData(user);
            return responseResultBuilder.Build();
        }

    }
}
