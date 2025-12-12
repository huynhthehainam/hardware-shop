

using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public sealed class TestController : ApiControllerBase
    {
        private readonly ITestService testService;
        public TestController(ITestService testService, IResponseResultBuilder responseResultBuilder) : base(responseResultBuilder)
        {
            this.testService = testService;
        }
        [HttpPost("TestWriteBack")]
        public async Task<IActionResult> TestWriteBack([FromBody] TestWriteBackCommand command)
        {
            await testService.TestWriteBackAsync();
            responseResultBuilder.SetData(new
            {
                Message = "Write-back test completed"
            });
            return responseResultBuilder.Build();
        }
        [HttpGet("TestEntity")]
        public async Task<IActionResult> TestEntity()
        {
            var data = await testService.TestEntityAsync();
            responseResultBuilder.SetData(new
            {
                Count = data
            });
            return responseResultBuilder.Build();
        }

        [HttpPost("StartSagaTest")]
        public async Task<IActionResult> StartSagaTest()
        {
            await testService.StartSagaTestAsync();
            responseResultBuilder.SetData(new
            {
                Message = "Saga test started"
            });
            return responseResultBuilder.Build();
        }
    }
}