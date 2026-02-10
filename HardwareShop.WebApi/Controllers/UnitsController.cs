


using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class UnitsController : AuthorizedApiControllerBase
    {
        private readonly IUnitService unitService;
        public UnitsController(IUnitService unitService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.unitService = unitService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUnit()
        {

            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/RoundValue")]
        public async Task<IActionResult> RoundValue([FromRoute] int id)
        {

            return responseResultBuilder.Build();
        }
        [HttpGet]
        public async Task<IActionResult> GetUnits([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] int? categoryId)
        {
            PageData<Application.Dtos.UnitDto> units = await unitService.GetUnitDtoPageDataAsync(pagingModel, search, categoryId);
            responseResultBuilder.SetPageData(units);
            return responseResultBuilder.Build();
        }
    }
}