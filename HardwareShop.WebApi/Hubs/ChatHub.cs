
using HardwareShop.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HardwareShop.WebApi.Hubs
{
    public static class ChatHubConstants
    {
        public const string Endpoint = "/chatHub";
    }
    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly IUserService userService;
        public ChatHub(IUserService userService)
        {
            this.userService = userService;
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("Connected");
            await base.OnConnectedAsync();
        }
        public async Task TestMe(string someRandomText)
        {
            var aa = Clients;
            await Clients.Others.SendAsync(
             $"ReceivedMessage", someRandomText);
        }
    }
}