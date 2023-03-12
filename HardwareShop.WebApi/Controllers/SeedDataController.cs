




using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class SeedDataController : AuthorizedApiControllerBase
    {
        public SeedDataController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
        }
        [HttpPost("SeedUnits")]
        public Task<IActionResult> SeedUnit()
        {
            return Task.FromResult(responseResultBuilder.Build());
        }
    }
}