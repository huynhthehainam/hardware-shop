﻿using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Helpers;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.ViewModels;
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
            var response = await userService.Login(command.Username ?? "", command.Password ?? "");
            if (response == null)
            {
                responseResultBuilder.AddInvalidFieldError("Username");
                responseResultBuilder.AddInvalidFieldError("Password");
                return responseResultBuilder.Build();
            }
            LoginViewModel vm = new LoginViewModel(response.AccessToken, new LoginUserViewModel(
                response.User.Role, new LoginUserDataViewModel(FullNameHelper.GetFullName(responseResultBuilder.GetConfiguration().Language, response.User.FirstName, response.User.LastName), response.User.Email, response.User.Settings)));

            responseResultBuilder.SetData(vm);

            return responseResultBuilder.Build();
        }
    }
}
