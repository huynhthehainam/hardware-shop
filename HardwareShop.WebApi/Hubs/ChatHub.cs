using HardwareShop.Core.Services;
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
        private readonly ICurrentUserService currentUserService;
        public ChatHub(ICurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
        }
        public override async Task OnConnectedAsync()
        {
            var userId = currentUserService.GetUserId();
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