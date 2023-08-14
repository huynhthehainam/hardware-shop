using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
