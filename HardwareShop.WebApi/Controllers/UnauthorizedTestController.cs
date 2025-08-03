
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public sealed class UnauthorizedTestController : ApiControllerBase
    {
        private readonly ITestService testService;

        public UnauthorizedTestController(ITestService testService, IResponseResultBuilder responseResultBuilder)
            : base(responseResultBuilder)
        {
            this.testService = testService;
        }

        [HttpGet("UnauthorizedTestEntity")]
        public async Task<IActionResult> UnauthorizedTestEntity(CancellationToken cancellationToken)
        {
            var users = await testService.TestEncryptedAsync(cancellationToken);
            return responseResultBuilder.SetData(users).Build();
        }
    }
}