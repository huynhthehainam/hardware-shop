using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IResponseResultFactory responseResultFactory;
        public HomeController(IResponseResultFactory responseResultFactory)
        {
            this.responseResultFactory = responseResultFactory;
        }
        public async Task<IActionResult> Index()
        {
            var response = responseResultFactory.Create();


            return response.ToResult();
        }
    }
}
