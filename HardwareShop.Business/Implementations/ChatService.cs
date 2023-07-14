

using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Business.Implementations
{
    public class ChatService : IChatService
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IShopService shopService;
        private readonly DbContext db;
        public ChatService(ICurrentUserService currentUserService, DbContext db, IShopService shopService)
        {
            this.shopService = shopService;
            this.db = db;
            this.currentUserService = currentUserService;
        }

        public async Task<List<ChatContactDto>?> GetContactsOfCurrentUserAsync()
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null) return null;
            var currentUerId = currentUserService.GetUserId();
            var users = db.Set<User>().Where(e => e.Id != currentUerId && e.UserShop != null && e.UserShop.ShopId == shop.Id).ToList();

            var groupSessions = db.Set<ChatSession>().Where(e => e.IsGroupChat && e.Members != null && e.Members.Any(e => e.UserId == currentUerId)).ToList();
            var finalContacts = new List<ChatContactDto>();
            var session = groupSessions.FirstOrDefault();
            if (session != null)
            {
                var count = db.Set<ChatMessage>().Count(e => e.SessionId == session.Id && e.UserId != currentUerId
                              && e.MessageStatuses != null && !e.MessageStatuses.Any(e => e.UserId == currentUerId && e.IsRead)
                               );
            }
            finalContacts.AddRange(groupSessions.Select(group => new ChatContactDto
            {
                Id = group.Id,
                AssetId = group.AssetId,
                IsGroupChat = true,
                Name = string.Join(", ", group.Members?.Select(e => e.User?.DisplayName ?? "") ?? new string[0]),
                Unread = db.Set<ChatMessage>().Count(e => e.SessionId == group.Id && e.UserId != currentUerId
                              && e.MessageStatuses != null && !e.MessageStatuses.Any(e => e.UserId == currentUerId && e.IsRead)
                               ),
                Status = "offline",
            }));




            var aa = users.Select(u => new ChatContactDto
            {
                AssetId = u.Assets?.FirstOrDefault(e => e.AssetType == UserAssetConstants.AvatarAssetType)?.AssetId ?? 0,
                Id = u.ChatSessionMembers?.FirstOrDefault(e => e.Session != null && !e.Session.IsGroupChat && e.Session.Members != null && e.Session.Members.Any(e => e.UserId == currentUerId))?.SessionId ?? 0,
                IsGroupChat = false,
                Name = u.DisplayName,
                Status = "offline",
                Unread = db.Set<ChatMessage>().Count(e => e.Member != null && e.Member.UserId == u.Id && e.Member.Session != null && !e.Member.Session.IsGroupChat && e.Member.Session.Members != null && e.Member.Session.Members.Any(e => e.UserId == currentUerId) && e.Member.Session.Members.Any(e => e.UserId == u.Id)
                 && e.MessageStatuses != null && !e.MessageStatuses.Any(e => e.UserId == currentUerId && e.IsRead)
                              ),
            }).ToList();

            finalContacts.AddRange(aa);
            return finalContacts;
        }
        public async Task<int?> CreateChatSessionAsync(List<int> userIds)
        {
            if (userIds.Count == 0) return null;

            var currentUerId = currentUserService.GetUserId();


            userIds.Add(currentUerId);
            userIds = userIds.Distinct().ToList();
            var shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null) return null;
            var validatedUsers = db.Set<User>().Where(e => e.UserShop != null && e.UserShop.ShopId == shop.Id && userIds.Contains(e.Id)).ToList();
            if (validatedUsers.Count != userIds.Count) return null;
            var existingSession = db.Set<ChatSession>().FirstOrDefault(e => e.Members != null && e.Members.All(m => userIds.Contains(m.UserId)));

            var createIfNotExistResponse = db.CreateIfNotExistsByQuery(new ChatSession
            {
                AssetId = validatedUsers.FirstOrDefault()?.Assets?.FirstOrDefault(e => e.AssetType == UserAssetConstants.AvatarAssetType)?.AssetId ?? 1,
                AssetType = ChatSessionAssetConstants.AvatarType,
                CreatedTime = DateTime.UtcNow,
                Members = validatedUsers.Select(e => new ChatSessionMember
                {
                    UserId = e.Id,

                }).ToList(),
                IsGroupChat = validatedUsers.Count > 2,
            }, e => e.Members != null && e.Members.All(m => userIds.Contains(m.UserId)));


            return createIfNotExistResponse.Entity.Id;
        }

        public async Task<long?> CreateChatMessage(int chatId, string message)
        {
            var chatSession = db.Set<ChatSession>().AsNoTracking().Where(e => e.Id == chatId).FirstOrDefault();
            if (chatSession == null) return null;
            var chatMessage = new ChatMessage()
            {
                Content = message,
                UserId = currentUserService.GetUserId(),
                SessionId = chatSession.Id,
            };
            db.Add(chatMessage);
            await db.SaveChangesAsync();
            return chatMessage.Id;
        }
        public async Task<PageData<ChatMessageDto>?> GetMessagesAsync(int chatId, PagingModel pagingModel)
        {
            var currentUserId = currentUserService.GetUserId();
            var session = await db.Set<ChatSession>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == chatId && e.Members != null && e.Members.Any(e => e.UserId == currentUserId));
            if (session == null) return null;
            var messages = db.Set<ChatMessage>().OrderByDescending(e => e.CreatedTime).GetPageData(pagingModel);
            return messages.ConvertToOtherPageData(e => new ChatMessageDto()
            {
                Content = e.Content,
                Id = e.Id,
                UserId = e.UserId,
            });

        }
    }
}