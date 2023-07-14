

using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface IChatService
    {
        Task<int?> CreateChatSessionAsync(List<int> userIds);
        Task<List<ChatContactDto>?> GetContactsOfCurrentUserAsync();
        Task<long?> CreateChatMessage(int chatId, string message);
        Task<PageData<ChatMessageDto>?> GetMessagesAsync(int chatId, PagingModel pagingModel);
    }
}