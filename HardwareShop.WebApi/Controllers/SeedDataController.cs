




using HardwareShop.Business.Dtos;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class SeedDataController : AuthorizedApiControllerBase
    {
        private readonly IRepository<Unit> unitRepository;
        public SeedDataController(IRepository<Unit> unitRepository, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.unitRepository = unitRepository;
        }
        [HttpPost("SeedUnits")]
        public async Task<IActionResult> SeedUnit([FromBody] List<SeedUnitCommand> commands)
        {
            List<UnitDto> unitDtos = new List<UnitDto>();
            foreach (var command in commands)
            {
                var unit = await unitRepository.CreateOrUpdateAsync(new Unit()
                {
                    Name = command.Name ?? "",
                    StepNumber = command.Step ?? 0.0,
                    UnitCategoryId = command.CategoryId ?? 0,

                }, e => new { e.Name }, e => new { e.Name });
                unitDtos.Add(new UnitDto() { Id = unit.Id, Name = unit.Name });
            }
            responseResultBuilder.SetData(unitDtos);
            return responseResultBuilder.Build();
        }
    }
}