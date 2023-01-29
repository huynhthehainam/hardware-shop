using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IResponseResultBuilder responseResultBuilder;
        public HomeController(IResponseResultBuilder responseResultBuilder)
        {
            this.responseResultBuilder = responseResultBuilder;
        }
        public Task<IActionResult> Index()
        {
            responseResultBuilder.SetData(new
            {
                Title = "My workshop"
            });
            return Task.FromResult(responseResultBuilder.Build());
        }
    }
}
