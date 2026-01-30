using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class CountriesController : AuthorizedApiControllerBase
    {
        public CountriesController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {

        }
        [HttpGet]
        public async Task<IActionResult> GetCountries([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/Icon")]
        public async Task<IActionResult> GetCountryIcon([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }
    }
}