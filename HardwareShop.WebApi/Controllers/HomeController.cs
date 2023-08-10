using HardwareShop.WebApi.Services;
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
            responseResultBuilder.SetData(new
            {
                Title = "My workshop"
            });
            return responseResultBuilder.Build();
        }
    }
}
