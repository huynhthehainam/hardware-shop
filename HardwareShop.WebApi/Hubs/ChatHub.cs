using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.BC;

namespace HardwareShop.WebApi.Hubs
{
    public static class ChatHubHelper
    {
        public static string GenerateGroupNameByUserId(int userId) => $"priv@tePers0n:{userId}";
        public static string GenerateGroupNameByShopId(int shopId) => $"cuRR3enTsh0P:{shopId}";
    }
    public static class ChatHubConstants
    {
        public const string Endpoint = "/chatHub";
    }
    public interface IChatHubController
    {
        bool CheckStatusByUserId(int userId);
        bool CheckStatusByUserIds(IEnumerable<int> userIds);
        List<string> GetConnectionIdsByUserIds(IEnumerable<int> userIds);
        string GetConnectionIdByUserId(int userId);
        void RemoveByUserId(int userId);
        void AddConnection(int userId, string connectionId);
    }
    public class ChatHubController : IChatHubController
    {
        private IDictionary<int, string> loggedConnections = new Dictionary<int, string>();
        public ChatHubController() { }

        public bool CheckStatusByUserId(int userId)
        {
            return loggedConnections.ContainsKey(userId);
        }
        public bool CheckStatusByUserIds(IEnumerable<int> userIds)
        {
            return userIds.Any(id => loggedConnections.ContainsKey(id));
        }
        public List<string> GetConnectionIdsByUserIds(IEnumerable<int> userIds)
        {
            return loggedConnections.Where(e => userIds.Contains(e.Key)).Select(e => e.Value).ToList();
        }
        public string GetConnectionIdByUserId(int userId)
        {
            return loggedConnections[userId].ToString();
        }

        public void RemoveByUserId(int userId)
        {
            lock (loggedConnections)
            {
                loggedConnections.Remove(userId);
            }
        }

        public void AddConnection(int userId, string connectionId)
        {
            lock (loggedConnections)
            {
                loggedConnections.Add(userId, connectionId);
            }
        }
    }
    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IChatService chatService;
        private readonly IShopService shopService;
        private readonly IChatHubController chatHubController;
        public ChatHub(ICurrentUserService currentUserService, IChatService chatService, IChatHubController chatHubController, IShopService shopService)
        {
            this.currentUserService = currentUserService;
            this.chatService = chatService;
            this.chatHubController = chatHubController;
            this.shopService = shopService;
        }
        public override async Task OnConnectedAsync()
        {
            var currentUserId = currentUserService.GetUserId();
            var connectionId = Context.ConnectionId;
            chatHubController.AddConnection(currentUserId, connectionId);

            await Groups.AddToGroupAsync(connectionId, ChatHubHelper.GenerateGroupNameByUserId(currentUserId));

            var chatSessions = await chatService.GetContactsOfCurrentUserAsync();
            var shop = shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop != null)
            {
                await Groups.AddToGroupAsync(connectionId, ChatHubHelper.GenerateGroupNameByShopId(shop.Id));
            }
            if (chatSessions != null)
            {
                foreach (var session in chatSessions)
                {
                    session.Status = chatHubController.CheckStatusByUserIds(session.Users.Where(e => e.UserId != currentUserId).Select(e => e.UserId)) ? "online" : "offline";
                }
                await Clients.Caller.SendAsync("InitContacts", chatSessions);
            }

            await Clients.Others.SendAsync("SomeOneConnected", new { UserId = currentUserId });

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var currentUserId = currentUserService.GetUserId();
            chatHubController.RemoveByUserId(currentUserId);
            await Clients.Others.SendAsync("SomeOneDisconnected", new { UserId = currentUserId });
            await base.OnDisconnectedAsync(exception);
        }


        public async Task JoinChatSession(List<int> userIds)
        {
            var createdChatSession = await chatService.CreateChatSessionAsync(userIds);
            if (createdChatSession != null)
            {
                if (createdChatSession.IsCreated)
                {
                    var connectionIds = chatHubController.GetConnectionIdsByUserIds(createdChatSession.AffectedUserIds);
                    var tasks = connectionIds.Select(connectionId => Clients.Client(connectionId).SendAsync("SomeoneCreatedChatSession", createdChatSession)).ToArray();
                    Task.WaitAll(tasks);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("UnsucessfullyCreatedChatSession");
            }
        }

        public async Task LoadMoreMessage(int sessionId, PagingModel pagingModel)
        {
            await Clients.Caller.SendAsync("GetMoreMessages", await chatService.GetMessagesAsync(sessionId, pagingModel));
        }

        public async Task SendChatMessage(int chatId, string msg)
        {
            if (chatId <= 0) return;
            var createdChatMessage = await chatService.CreateChatMessageAsync(chatId, msg);
            if (createdChatMessage != null)
            {
                var connectionIds = chatHubController.GetConnectionIdsByUserIds(createdChatMessage.AffectedUsers.Select(e => e.UserId));
                var tasks = connectionIds.Select(connectionId => Clients.Client(connectionId).SendAsync("SomeoneSentMessage", createdChatMessage)).ToArray();
                Task.WaitAll(tasks);
            }
        }
    }
}