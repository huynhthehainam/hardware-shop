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
        public UsersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {

        }

        [HttpGet("Me/GetAvatar")]
        public async Task<IActionResult> GetAvatar()
        {
            return responseResultBuilder.Build();
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/UpdateInterfaceSettings")]
        public async Task<IActionResult> UpdateCurrentUserSettings([FromBody] UpdateInterfaceSettingsCommand command)
        {
            return responseResultBuilder.Build();
        }


        [HttpGet("Me/Notifications")]
        public async Task<IActionResult> GetCurrentUserNotifications([FromQuery] PagingModel pagingModel)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/Notifications")]
        public async Task<IActionResult> CreateCurrentUserNotification([FromBody] CreateNotificationCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/Notifications/{id:Guid}/Dismiss")]
        public async Task<IActionResult> DismissNotification([FromRoute] Guid id)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("Me/DismissAllNotifications")]
        public async Task<IActionResult> DismissAllNotifications()
        {
            return responseResultBuilder.Build();
        }
    }
}
