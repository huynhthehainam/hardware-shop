using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class CountriesController : AuthorizedApiControllerBase
    {
        private readonly ICountryService countryService;
        public CountriesController(ICountryService countryService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.countryService = countryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCountries([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var countriesResponse = await countryService.GetCountryPageData(pagingModel, search);
            responseResultBuilder.SetApplicationResponse(countriesResponse, (builder, result) =>
            {
                builder.SetPageData(result);
            });
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/Icon")]
        public async Task<IActionResult> GetCountryIcon([FromRoute] int id)
        {
            var assetResponse = await countryService.GetCountryIconByIdAsync(id);
            responseResultBuilder.SetApplicationResponse(assetResponse, (builder, result) =>
            {

                builder.SetAsset(result);
            });
            return responseResultBuilder.Build();
        }
    }
}