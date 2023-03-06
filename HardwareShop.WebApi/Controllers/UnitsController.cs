


using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
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
        public async Task<IActionResult> CreateUnit([FromBody] CreateUnitCommand command)
        {
            var unit = await unitService.CreateUnit(command.Name ?? "", command.StepNumber ?? 0, command.UnitCategoryId ?? 0);
            if (unit == null) return responseResultBuilder.Build();
            responseResultBuilder.SetData(unit);
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/RoundValue")]
        public async Task<IActionResult> RoundValue([FromRoute] int id, [FromBody] RoundNumberCommand command)
        {
            var newValue = await unitService.RoundValue(id, command.Value.GetValueOrDefault());
            if (newValue == null) return responseResultBuilder.Build();
            responseResultBuilder.SetData(newValue);
            return responseResultBuilder.Build();
        }
        [HttpGet]
        public async Task<IActionResult> GetUnits([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] int? categoryId)
        {
            var units = await unitService.GetUnitDtoPageDataAsync(pagingModel, search, categoryId);
            responseResultBuilder.SetPageData(units);
            return responseResultBuilder.Build();
        }
    }
}