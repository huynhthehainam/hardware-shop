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
        public async Task<IActionResult> Index()
        {



            return responseResultBuilder.Build();
        }
    }
}
