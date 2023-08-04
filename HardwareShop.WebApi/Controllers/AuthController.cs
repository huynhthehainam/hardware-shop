using HardwareShop.Application.Services;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public sealed class AuthController : ApiControllerBase
    {

        private readonly IUserService userService;
        public AuthController(IResponseResultBuilder responseResultBuilder, IUserService userService) : base(responseResultBuilder)
        {
            this.userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await userService.LoginAsync(command.Username ?? "", command.Password ?? "");
            if (response == null)
            {
                responseResultBuilder.AddInvalidFieldError("Username");
                responseResultBuilder.AddInvalidFieldError("Password");
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetData(response);

            return responseResultBuilder.Build();
        }
        [HttpPost("LoginByToken")]
        public async Task<IActionResult> LoginByToken([FromBody] LoginByTokenCommand command)
        {
            var response = await userService.LoginByTokenAsync(command.Token ?? "");
            if (response == null)
            {
                responseResultBuilder.AddInvalidFieldError("Token");
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetData(response);

            return responseResultBuilder.Build();
        }
    }
}
