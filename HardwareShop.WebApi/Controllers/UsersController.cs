using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
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
    }
}
