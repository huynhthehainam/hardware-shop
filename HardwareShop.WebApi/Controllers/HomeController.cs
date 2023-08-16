using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class TestViewModel
    {
        public string Label { get; set; } = string.Empty;
    }
    public class HomeController : ControllerBase
    {
        private readonly IResponseResultBuilder responseResultBuilder;

        public HomeController(IResponseResultBuilder responseResultBuilder)
        {
            this.responseResultBuilder = responseResultBuilder;

        }
        public IActionResult Index()
        {
            responseResultBuilder.SetData(new
            {
                Title = "My workshop"
            });
            return responseResultBuilder.Build();
        }
    }
}
