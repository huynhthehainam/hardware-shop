


using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface IChatService
    {
        Task<CreatedChatSessionDto?> CreateChatSessionAsync(List<Guid> userIds);
        Task<List<ChatContactDto>?> GetContactsOfCurrentUserAsync();
        Task<CreatedChatMessageDto?> CreateChatMessageAsync(int chatId, string message);
        Task<PageData<ChatMessageDto>> GetMessagesAsync(int chatId, PagingModel pagingModel);
        Task<bool> MarkAsReadForCurrentUserAsync(int chatSessionId);
    }
}