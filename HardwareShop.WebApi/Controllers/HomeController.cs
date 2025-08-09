using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HardwareShop.WebApi.Controllers
{
    public class TestViewModel
    {
        public string Label { get; set; } = string.Empty;
    }
    public class HomeController : ControllerBase
    {
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IStringLocalizer<HomeController> localizer;

        public HomeController(IResponseResultBuilder responseResultBuilder, IStringLocalizer<HomeController> localizer)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.localizer = localizer;

        }
        public IActionResult Index()
        {
            responseResultBuilder.SetData(new
            {
                Title = "My workshop",
                Content = localizer["Content", "Nam Huynh"].Value
            });
            return responseResultBuilder.Build();
        }
    }
}
