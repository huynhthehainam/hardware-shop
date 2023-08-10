


using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Models;

namespace HardwareShop.Application.Services
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