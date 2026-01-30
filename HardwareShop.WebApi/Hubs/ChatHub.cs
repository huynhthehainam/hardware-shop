
using System.Diagnostics;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HardwareShop.WebApi.Hubs
{
    public static class ChatHubHelper
    {
        public static string GenerateGroupNameByUserId(Guid guid) => $"priv@tePers0n:{guid}";
        public static string GenerateGroupNameByShopId(int shopId) => $"cuRR3enTsh0P:{shopId}";
    }

    public static class ChatHubConstants
    {
        public const string Endpoint = "/chatHub";
    }
    public interface IChatHubController
    {
        bool CheckStatusByUserId(Guid userId);
        bool CheckStatusByUserIds(IEnumerable<Guid> userIds);

        void RemoveByUserId(Guid userId, string connectionId);
        void AddConnection(Guid userId, string connectionId);
    }
    public class ChatHubController : IChatHubController
    {
        private IDictionary<Guid, List<string>> loggedConnections = new Dictionary<Guid, List<string>>();
        private readonly object padLock = new object();
        public ChatHubController()
        {

        }

        public bool CheckStatusByUserId(Guid userId)
        {
            return loggedConnections.ContainsKey(userId);
        }
        public bool CheckStatusByUserIds(IEnumerable<Guid> userIds)
        {
            return userIds.Any(id => loggedConnections.ContainsKey(id));
        }


        public void RemoveByUserId(Guid userId, string connectionId)
        {
            lock (padLock)
            {
                if (loggedConnections.ContainsKey(userId))
                {
                    loggedConnections[userId].Remove(connectionId);
                }
            }
        }

        public void AddConnection(Guid userId, string connectionId)
        {
            lock (padLock)
            {
                if (loggedConnections.ContainsKey(userId))
                {
                    loggedConnections[userId].Add(connectionId);
                }
                else
                {
                    loggedConnections.Add(userId, new List<string> { connectionId });
                }

            }
        }


    }
    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IChatHubController chatHubController;
        public ChatHub(ICurrentUserService currentUserService, IChatHubController chatHubController)
        {
            Debug.WriteLine("Chat hub is established");
            this.currentUserService = currentUserService;
            this.chatHubController = chatHubController;
        }
        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Connection {Context.ConnectionId} is established");
            var currentUserGuid = currentUserService.GetUserGuid();
            var connectionId = Context.ConnectionId;
            chatHubController.AddConnection(currentUserGuid, connectionId);

            await Groups.AddToGroupAsync(connectionId, ChatHubHelper.GenerateGroupNameByUserId(currentUserGuid));




            await Clients.Others.SendAsync("SomeOneConnected", new { UserId = currentUserGuid });

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var currentUserGuid = currentUserService.GetUserGuid();
            Debug.WriteLine($"Connection {Context.ConnectionId} is removed");
            chatHubController.RemoveByUserId(currentUserGuid, Context.ConnectionId);
            await Clients.Others.SendAsync("SomeOneDisconnected", new { UserGuid = currentUserGuid });
            await base.OnDisconnectedAsync(exception);
        }

        private async Task SendMultipleUserIds(IEnumerable<Guid> userGuids, Func<IClientProxy, Task> sendAction)
        {
            var tasks = userGuids.Select(guid => sendAction(Clients.Group(ChatHubHelper.GenerateGroupNameByUserId(guid))));
            await Task.WhenAll(tasks);
        }
        public async Task JoinChatSession(List<Guid> userGuids)
        {

        }

        public async Task LoadMoreMessage(int sessionId, PagingModel pagingModel)
        {
        }

        public async Task SendChatMessage(int sessionId, string msg)
        {

        }
    }
}