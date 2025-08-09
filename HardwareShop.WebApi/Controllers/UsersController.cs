using System.Text.Json;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
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

        [HttpGet("Me/GetAvatar")]
        public async Task<IActionResult> GetAvatar()
        {
            var response = await userService.GetCurrentUserAvatarAsync();
            responseResultBuilder.SetApplicationResponse(response, (builder, result) =>
            {
                builder.SetAsset(result);
            });
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
        [HttpPost("Me/UpdateInterfaceSettings")]
        public async Task<IActionResult> UpdateCurrentUserSettings([FromBody] UpdateInterfaceSettingsCommand command)
        {
            var response = await userService.UpdateCurrentUserInterfaceSettings(command.Settings ?? JsonDocument.Parse("{}"));
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }


        [HttpPost("Me/UpdatePassword")]
        public async Task<IActionResult> UpdateCurrentUserPassword([FromBody] UpdatePasswordCommand command)
        {
            var response = await userService.UpdateCurrentUserPasswordAsync(command.OldPassword ?? "", command.NewPassword ?? "");
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }
        [HttpGet("Me/Notifications")]
        public async Task<IActionResult> GetCurrentUserNotifications([FromQuery] PagingModel pagingModel)
        {
            var response = await userService.GetNotificationDtoPageDataOfCurrentUserAsync(pagingModel);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) =>
            {
                builder.SetPageData(result);
            });
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/Notifications")]
        public async Task<IActionResult> CreateCurrentUserNotification([FromBody] CreateNotificationCommand command)
        {
            var notification = await userService.CreateNotificationOfCurrentUserAsync(command.Message, command.Variant ?? "", command.Translation, command.TranslationParams);
            if (notification == null) return responseResultBuilder.Build();

            responseResultBuilder.SetData(notification);
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/Notifications/{id:Guid}/Dismiss")]
        public async Task<IActionResult> DismissNotification([FromRoute] Guid id)
        {
            var response = await userService.DismissNotificationOfCurrentUserAsync(id);
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/DismissAllNotifications")]
        public async Task<IActionResult> DismissAllNotifications()
        {
            var response = await userService.DismissAllNotificationsOfCurrentUserAsync();
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }
    }
}
