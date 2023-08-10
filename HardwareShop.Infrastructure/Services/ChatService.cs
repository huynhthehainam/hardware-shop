

using System.Linq.Expressions;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Services
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
            var currentUserId = currentUserService.GetUserGuid();
            var currentUserGuid = currentUserService.GetUserGuid();
            var users = db.Set<User>().Where(e => e.Guid != currentUserId && e.UserShop != null && e.UserShop.ShopId == shop.Id).ToList();

            var groupSessions = db.Set<ChatSession>().Where(e => e.IsGroupChat && e.Members != null && e.Members.Any(e => e.User!.Guid == currentUserId)).ToList();
            var finalContacts = new List<ChatContactDto>();
            var session = groupSessions.FirstOrDefault();

            finalContacts.AddRange(groupSessions.Select(group => new ChatContactDto
            {
                Id = group.Id,
                AssetId = group.AssetId,
                IsGroupChat = true,
                Name = string.IsNullOrEmpty(group.Name) ? string.Join(", ", group.Members?.Select(e => e.User?.DisplayName ?? "") ?? new string[0] { }) : group.Name,
                Unread = GetUnreadOfChatSessionByUserId(group.Id, currentUserId),
                Status = "offline",
                AffectedUserIds = group.Members?.Where(e => e.User!.Guid != currentUserId).Select(e => e.User?.Guid ?? Guid.NewGuid()).ToList() ?? new List<Guid>(),
                Users = group.Members?.Select(e => new ContactUserDto
                {
                    UserGuid = e.User?.Guid ?? Guid.NewGuid(),
                    AssetId = e.User?.GetAvatarAssetId() ?? 0
                }).ToArray() ?? new ContactUserDto[0],
            }));




            var userSessions = users.Select(u => new ChatContactDto
            {
                AssetId = u.GetAvatarAssetId() ?? 0,
                Id = u.ChatSessionMembers?.FirstOrDefault(e => e.Session != null && !e.Session.IsGroupChat && e.Session.Members != null && e.Session.Members.Any(e => e.User!.Guid == currentUserId))?.SessionId ?? 0,
                IsGroupChat = false,
                Name = u.DisplayName,
                Status = "offline",
                Unread = db.Set<ChatMessage>().Count(e => e.Member != null && e.Member.UserId == u.Id && e.Member.Session != null && !e.Member.Session.IsGroupChat && e.Member.Session.Members != null && e.Member.Session.Members.Any(e => e.User!.Guid == currentUserId) && e.Member.Session.Members.Any(e => e.UserId == u.Id)
                 && e.MessageStatuses != null && !e.MessageStatuses.Any(e => e.User!.Guid == currentUserId && e.IsRead)
                              ),
                AffectedUserIds = new List<Guid> { u.Guid },
                Users = new ContactUserDto[] { new ContactUserDto { AssetId = u.GetAvatarAssetId() ?? 0, UserGuid = u.Guid }, new ContactUserDto { UserGuid = currentUserGuid, AssetId = 0 } },
            }).ToList();

            finalContacts.AddRange(userSessions);
            return finalContacts;
        }
        private int GetUnreadOfChatSessionByUserId(int sessionId, Guid userId)
        {
            return db.Set<ChatMessage>().Count(GetUnreadChatSessionExpression(sessionId, userId));
        }

        private Expression<Func<ChatMessage, bool>> GetUnreadChatSessionExpression(int sessionId, Guid userId)
        {
            return e => e.SessionId == sessionId && e.User!.Guid != userId
                                  && e.MessageStatuses != null && !e.MessageStatuses.Any(e => e.User!.Guid == userId && e.IsRead);
        }
        public async Task<CreatedChatSessionDto?> CreateChatSessionAsync(List<Guid> userGuids)
        {
            var affectedUserIds = userGuids;
            if (userGuids.Count == 0) return null;

            var currentUerGuid = currentUserService.GetUserGuid();


            userGuids.Add(currentUerGuid);
            userGuids = userGuids.Distinct().ToList();
            var shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null) return null;
            var validatedUsers = db.Set<User>().Where(e => e.UserShop != null && e.UserShop.ShopId == shop.Id && userGuids.Contains(e.Guid)).ToList();
            if (validatedUsers.Count != userGuids.Count) return null;
            var validatedUserIds = validatedUsers.Select(e => e.Id).ToList();
            var existingSession = db.Set<ChatSession>().FirstOrDefault(e => e.Members != null && e.Members.All(m => validatedUserIds.Contains(m.UserId)));

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
            }, e => e.Members != null && e.Members.All(m => validatedUserIds.Contains(m.UserId)));

            var messages = await GetMessagesAsync(createIfNotExistResponse.Entity.Id, new PagingModel { PageIndex = 0, PageSize = 20 });
            return new CreatedChatSessionDto
            {
                AffectedUserIds = affectedUserIds,
                Id = createIfNotExistResponse.Entity.Id,
                IsCreated = !createIfNotExistResponse.IsExist,
                IsGroupChat = createIfNotExistResponse.Entity.IsGroupChat,
                CreatedUserGuid = currentUerGuid,
                Status = "offline",
                Unread = 0,
                AssetId = createIfNotExistResponse.Entity.AssetId,
                Users = validatedUsers.Select(e => new ContactUserDto { UserGuid = e.Guid, AssetId = e.GetAvatarAssetId() ?? 0 }).ToArray(),
                Messages = messages,
            };
        }

        public async Task<CreatedChatMessageDto?> CreateChatMessageAsync(int chatId, string message)
        {
            var chatSession = db.Set<ChatSession>().Include(e => e.Members!).ThenInclude(e => e.User).Where(e => e.Id == chatId).FirstOrDefault();
            if (chatSession == null) return null;
            var currentUserGuid = currentUserService.GetUserGuid();
            var currentUserId = db.Set<User>().AsNoTracking().Single(e => e.Guid == currentUserGuid).Id;
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
                AffectedUsers = chatSession.Members?.Select(e => new ChatAffectedUser { UserGuid = e.User?.Guid ?? Guid.Empty, Unread = GetUnreadOfChatSessionByUserId(chatSession.Id, e.User!.Guid) }).ToList() ?? new List<ChatAffectedUser>(),
                ChatSessionId = chatSession.Id,
                CreatedUseGuid = currentUserGuid,
                Message = chatMessage.Content,
            };
        }
        public async Task<PageData<ChatMessageDto>> GetMessagesAsync(int chatSessionId, PagingModel pagingModel)
        {
            var currentUserId = currentUserService.GetUserGuid();

            var messagePageData = await db.Set<ChatMessage>().Where(e => e.SessionId == chatSessionId).Include(e => e.User).OrderByDescending(e => e.CreatedTime).GetPageDataAsync(pagingModel);
            messagePageData.Items = messagePageData.Items.OrderBy(e => e.CreatedTime).ToArray();
            return messagePageData.ConvertToOtherPageData(e => new ChatMessageDto()
            {
                Content = e.Content,
                Id = e.Id,
                UserGuid = e.User?.Guid ?? Guid.Empty,
                Time = e.CreatedTime,
                IsRead = e.MessageStatuses != null && e.MessageStatuses.Any(e => e.User!.Guid == currentUserId && e.IsRead)
            });
        }
        public async Task<bool> MarkAsReadForCurrentUserAsync(int chatSessionId)
        {
            using var transaction = db.Database.BeginTransaction();
            try
            {
                var currentUserGuid = currentUserService.GetUserGuid();
                var currentUserId = db.Set<User>().AsNoTracking().Select(e => e.Id).Single();
                var unreadMessageIds = await db.Set<ChatMessage>().Where(GetUnreadChatSessionExpression(chatSessionId, currentUserGuid)).Select(e => e.Id).ToListAsync();
                var statuses = db.Set<ChatMessageStatus>().Where(e => unreadMessageIds.Contains(e.MessageId) && e.User!.Guid == currentUserGuid).ToList();
                foreach (var messageId in unreadMessageIds)
                {
                    var currentStatus = statuses.FirstOrDefault(e => e.MessageId == messageId);
                    if (currentStatus != null)
                    {
                        currentStatus.IsRead = true;
                    }
                    else
                    {
                        db.Set<ChatMessageStatus>().Add(new ChatMessageStatus
                        {
                            UserId = currentUserId,
                            CreatedTime = DateTime.UtcNow,
                            IsRead = true,
                            MessageId = messageId,
                            SessionId = chatSessionId,

                        });
                    }
                }
                db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}