using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public sealed class AuthController : ApiControllerBase
    {

        private readonly IAccountService accountService;
        public AuthController(IResponseResultBuilder responseResultBuilder, IAccountService accountService) : base(responseResultBuilder)
        {
            this.accountService = accountService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await accountService.Login(command.Username ?? "", command.Password ?? "");
            if (response == null)
            {
                responseResultBuilder.AddInvalidFieldError("Username");
                responseResultBuilder.AddInvalidFieldError("Password");
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetData(response);

            return responseResultBuilder.Build();
        }
    }
}
