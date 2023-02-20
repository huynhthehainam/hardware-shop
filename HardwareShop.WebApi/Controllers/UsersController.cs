using System.Text.Json;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
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
        public async Task<IActionResult> GetUsers([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            if (!currentUserService.IsSystemAdmin())
            {
                responseResultBuilder.AddNotPermittedError();
                return responseResultBuilder.Build();
            }

            var users = await userService.GetUserPageDataAsync(pagingModel, search);
            responseResultBuilder.SetPageData(users);
            return responseResultBuilder.Build();
        }
        [HttpPost("me/UpdateInterfaceSettings")]
        public async Task<IActionResult> UpdateCurrentUserSettings([FromBody] UpdateInterfaceSettingsCommand command)
        {
            var isSuccess = await userService.UpdateCurrentUserInterfaceSettings(command.Settings ?? JsonDocument.Parse("{}"));
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
    }
}
