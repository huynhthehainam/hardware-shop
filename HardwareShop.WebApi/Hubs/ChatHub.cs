
using System.Diagnostics;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
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
        private readonly IChatService chatService;
        private readonly IShopService shopService;
        private readonly IChatHubController chatHubController;
        public ChatHub(ICurrentUserService currentUserService, IChatService chatService, IChatHubController chatHubController, IShopService shopService)
        {
            Debug.WriteLine("Chat hub is established");
            this.currentUserService = currentUserService;
            this.chatService = chatService;
            this.chatHubController = chatHubController;
            this.shopService = shopService;
        }
        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Connection {Context.ConnectionId} is established");
            var currentUserGuid = currentUserService.GetUserGuid();
            var connectionId = Context.ConnectionId;
            chatHubController.AddConnection(currentUserGuid, connectionId);

            await Groups.AddToGroupAsync(connectionId, ChatHubHelper.GenerateGroupNameByUserId(currentUserGuid));
            var userGuids = new List<string>();

            var shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop != null)
            {
                await Groups.AddToGroupAsync(connectionId, ChatHubHelper.GenerateGroupNameByShopId(shop.Id));
            }
            var chatSessions = await chatService.GetContactsOfCurrentUserAsync();
            if (chatSessions != null)
            {
                foreach (var session in chatSessions)
                {
                    session.Status = chatHubController.CheckStatusByUserIds(session.Users.Where(e => e.UserGuid != currentUserGuid).Select(e => e.UserGuid)) ? "online" : "offline";
                }
                await Clients.Caller.SendAsync("InitContacts", chatSessions);
            }

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
            var createdChatSession = await chatService.CreateChatSessionAsync(userGuids);
            if (createdChatSession != null)
            {
                if (createdChatSession.IsCreated)
                {
                    await SendMultipleUserIds(createdChatSession.AffectedUserIds, client => client.SendAsync("SomeoneCreatedChatSession", createdChatSession));
                }
                else
                {
                    var isSuccess = await chatService.MarkAsReadForCurrentUserAsync(createdChatSession.Id);
                    if (isSuccess)
                    {
                        await SendMultipleUserIds(new List<Guid> { createdChatSession.CreatedUserGuid }, client =>
                            client.SendAsync("OtherDeviceReadAllMessage", new { ChatSessionId = createdChatSession.Id })
                        );
                    }
                    await Clients.Caller.SendAsync("SuccessfullyCreatedChatSession", createdChatSession);
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

        public async Task SendChatMessage(int sessionId, string msg)
        {
            if (sessionId <= 0) return;
            CreatedChatMessageDto? createdChatMessage = await chatService.CreateChatMessageAsync(sessionId, msg);
            if (createdChatMessage != null)
            {
                await SendMultipleUserIds(createdChatMessage.AffectedUsers.Select(e => e.UserGuid), client => client.SendAsync("SomeoneSentMessage", createdChatMessage));
            }
        }
    }
}