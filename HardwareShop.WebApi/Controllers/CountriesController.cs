


using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
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
            var countries = await countryService.GetCountryPageData(pagingModel, search);
            responseResultBuilder.SetPageData(countries);
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/Icon")]
        public async Task<IActionResult> GetCountryIcon([FromRoute] int id)
        {
            var asset = await countryService.GetCountryIconByIdAsync(id);
            if (asset == null) return responseResultBuilder.Build();
            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
    }
}