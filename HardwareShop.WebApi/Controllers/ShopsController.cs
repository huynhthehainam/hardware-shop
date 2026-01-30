using HardwareShop.Application.CQRS.ShopArea.Commands;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Extensions;
using HardwareShop.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ShopsController : AuthorizedApiControllerBase
    {

        private readonly IMediator mediator;
        public ShopsController(IMediator mediator, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetShops([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
        {
            var response = await mediator.Send(command);
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateLogo")]
        public async Task<IActionResult> UpdateLogo([FromRoute] int id, [FromForm] UpdateShopLogoCommand command)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost("YourShop/UpdateLogo")]
        public async Task<IActionResult> UpdateYourShopLogo([FromForm] UpdateShopLogoCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UpdateSetting")]
        public async Task<IActionResult> UpdateShopSettings([FromRoute] int id, [FromBody] UpdateShopSettingCommand command)
        {
            return responseResultBuilder.Build();
        }

        [HttpGet("YourShop/Logo")]
        public async Task<IActionResult> GetYourShopLogo()
        {
            return responseResultBuilder.Build();
        }


        [HttpGet("YourShop/Users")]
        public async Task<IActionResult> GetUsersOfYourShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            return responseResultBuilder.Build();
        }


        [HttpPost("{id:int}/DeleteSoftly")]
        public async Task<IActionResult> DeleteShopSoftly([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost("{id:int}/CreateAdminUser")]
        public async Task<IActionResult> CreateAdminUser([FromRoute] int id, CreateShopAdminUserCommand command)
        {
            return responseResultBuilder.Build();
        }

    }
}
