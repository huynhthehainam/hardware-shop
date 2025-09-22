using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;

namespace HardwareShop.Infrastructure.Services;

public class ChatService : IChatService
{
    public Task<CreatedChatMessageDto?> CreateChatMessageAsync(int chatId, string message)
    {
        throw new NotImplementedException();
    }

    public Task<CreatedChatSessionDto?> CreateChatSessionAsync(List<Guid> userIds)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MarkAsReadForCurrentUserAsync(int chatSessionId)
    {
        throw new NotImplementedException();
    }

    public Task<PageData<ChatMessageDto>> GetMessagesAsync(int chatId, PagingModel pagingModel)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatContactDto>?> GetContactsOfCurrentUserAsync()
    {
        throw new NotImplementedException();
    }
}