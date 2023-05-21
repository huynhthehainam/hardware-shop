using System.Text.Json;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Extensions;
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

        [HttpGet("Me/GetAvatar")]
        public async Task<IActionResult> GetAvatar()
        {
            var asset = await userService.GetCurrentUserAvatarAsync();
            if (asset == null)
            {
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
        [HttpPost("Me/UpdateInterfaceSettings")]
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


        [HttpPost("Me/UpdatePassword")]
        public async Task<IActionResult> UpdateCurrentUserPassword([FromBody] UpdatePasswordCommand command)
        {
            var isSuccess = await userService.UpdateCurrentUserPasswordAsync(command.OldPassword ?? "", command.NewPassword ?? "");
            if (!isSuccess) return responseResultBuilder.Build();

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
        [HttpGet("Me/Notifications")]
        public async Task<IActionResult> GetCurrentUserNotifications([FromQuery] PagingModel pagingModel)
        {
            var notifications = await userService.GetNotificationDtoPageDataOfCurrentUserAsync(pagingModel);
            if (notifications == null) return responseResultBuilder.Build();
            responseResultBuilder.SetPageData(notifications);
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
            bool isSuccess = await userService.DismissNotificationOfCurrentUserAsync(id);
            if (!isSuccess) return responseResultBuilder.Build();
            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/DismissAllNotifications")]
        public async Task<IActionResult> DismissAllNotifications()
        {
            bool isSuccess = await userService.DismissAllNotificationsOfCurrentUserAsync();
            if (!isSuccess) return responseResultBuilder.Build();
            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
    }
}
