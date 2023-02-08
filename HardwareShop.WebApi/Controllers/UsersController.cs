using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class UsersController : AuthorizedApiControllerBase
    {
        private readonly IUserService userService;


        public UsersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, IUserService userService) : base(responseResultBuilder, currentUserService)
        {
            this.userService = userService;
        }

        [HttpGet("me/GetAvatar")]
        public async Task<IActionResult> GetAvatar()
        {
            var asset = await userService.GetCurrentUserAvatarAsync();
            if (asset == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Avatar");
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
        [HttpGet]
        public async Task<IActionResult> GetUsersOfShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            PageData<UserDto>? users = await userService.GetUsersOfShopAsync(pagingModel, search);
            if (users == null) return responseResultBuilder.Build();

            responseResultBuilder.SetPageData(users);
            return responseResultBuilder.Build();
        }
    }
}
