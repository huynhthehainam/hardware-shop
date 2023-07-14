

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

            finalContacts.AddRange(groupSessions.Select(group => new ChatContactDto
            {
                Id = group.Id,
                AssetId = group.AssetId,
                IsGroupChat = true,
                Name = string.IsNullOrEmpty(group.Name) ? string.Join(", ", group.Members?.Select(e => e.User?.DisplayName ?? "") ?? new string[0] { }) : group.Name,
                Unread = GetUnreadOfChatSessionByUserId(group.Id, currentUerId),
                Status = "offline",
                Users = group.Members?.Select(e => new ContactUserDto
                {
                    UserId = e.UserId,
                    AssetId = e.User?.GetAvatarAssetId() ?? 0
                }).ToArray() ?? new ContactUserDto[0],
            }));




            var userSessions = users.Select(u => new ChatContactDto
            {
                AssetId = u.GetAvatarAssetId() ?? 0,
                Id = u.ChatSessionMembers?.FirstOrDefault(e => e.Session != null && !e.Session.IsGroupChat && e.Session.Members != null && e.Session.Members.Any(e => e.UserId == currentUerId))?.SessionId ?? 0,
                IsGroupChat = false,
                Name = u.DisplayName,
                Status = "offline",
                Unread = db.Set<ChatMessage>().Count(e => e.Member != null && e.Member.UserId == u.Id && e.Member.Session != null && !e.Member.Session.IsGroupChat && e.Member.Session.Members != null && e.Member.Session.Members.Any(e => e.UserId == currentUerId) && e.Member.Session.Members.Any(e => e.UserId == u.Id)
                 && e.MessageStatuses != null && !e.MessageStatuses.Any(e => e.UserId == currentUerId && e.IsRead)
                              ),
                Users = new ContactUserDto[] { new ContactUserDto { AssetId = u.GetAvatarAssetId() ?? 0, UserId = u.Id }, new ContactUserDto { UserId = currentUerId, AssetId = 0 } },
            }).ToList();

            finalContacts.AddRange(userSessions);
            return finalContacts;
        }
        private int GetUnreadOfChatSessionByUserId(int sessionId, int userId)
        {
            return db.Set<ChatMessage>().Count(e => e.SessionId == sessionId && e.UserId != userId
                                  && e.MessageStatuses != null && !e.MessageStatuses.Any(e => e.UserId == userId && e.IsRead)
                                   );
        }
        public async Task<CreatedChatSessionDto?> CreateChatSessionAsync(List<int> userIds)
        {
            var affectedUserIds = userIds;
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


            return new CreatedChatSessionDto
            {
                AffectedUserIds = affectedUserIds,
                Id = createIfNotExistResponse.Entity.Id,
                IsCreated = !createIfNotExistResponse.IsExist,
                IsGroupChat = createIfNotExistResponse.Entity.IsGroupChat,
                CreatedUserId = currentUerId,
                Status = "online",
                Unread = 0,
                AssetId = createIfNotExistResponse.Entity.AssetId,
                Users = validatedUsers.Select(e => new ContactUserDto { UserId = e.Id, AssetId = e.GetAvatarAssetId() ?? 0 }).ToArray(),
                Messages = await GetMessagesAsync(createIfNotExistResponse.Entity.Id, new PagingModel { PageIndex = 0, PageSize = 20 }),
            };
        }

        public async Task<CreatedChatMessageDto?> CreateChatMessageAsync(int chatId, string message)
        {
            var chatSession = db.Set<ChatSession>().AsNoTracking().Include(e => e.Members).Where(e => e.Id == chatId).FirstOrDefault();
            if (chatSession == null) return null;
            var currentUserId = currentUserService.GetUserId();
            var chatMessage = new ChatMessage()
            {
                Content = message,
                UserId = currentUserId,
                SessionId = chatSession.Id,
            };
            db.Add(chatMessage);
            await db.SaveChangesAsync();
            return new CreatedChatMessageDto
            {
                AffectedUsers = chatSession.Members?.Select(e => new ChatAffectedUser { UserId = e.UserId, Unread = GetUnreadOfChatSessionByUserId(chatSession.Id, e.UserId) }).ToList() ?? new List<ChatAffectedUser>(),
                ChatSessionId = chatSession.Id,
                CreatedUserId = currentUserId,
                Message = chatMessage.Content,
            };
        }
        public async Task<PageData<ChatMessageDto>> GetMessagesAsync(int chatSessionId, PagingModel pagingModel)
        {
            var currentUserId = currentUserService.GetUserId();

            var messages = await db.Set<ChatMessage>().Where(e => e.SessionId == chatSessionId).OrderByDescending(e => e.CreatedTime).AsNoTracking().GetPageDataAsync(pagingModel);
            return messages.ConvertToOtherPageData(e => new ChatMessageDto()
            {
                Content = e.Content,
                Id = e.Id,
                UserId = e.UserId,
            });
        }
    }
}