using HardwareShop.Core.Services;
using HardwareShop.WebApi.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HardwareShop.WebApi.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHubContext<ChatHub> chatHubContext;
        public HomeController(IResponseResultBuilder responseResultBuilder, IHubContext<ChatHub> chatHubContext)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.chatHubContext = chatHubContext;
        }
        public async Task<IActionResult> Index()
        {
            await chatHubContext.Clients.All.SendAsync("TestMe", "asfasfasf");
            responseResultBuilder.SetData(new
            {
                Title = "My workshop"
            });
            return responseResultBuilder.Build();
        }
    }
}
