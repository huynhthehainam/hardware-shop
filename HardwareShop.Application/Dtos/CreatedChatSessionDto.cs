using HardwareShop.Application.Models;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Dtos
{
    public sealed class CreatedChatSessionDto : ChatContactDto
    {

        public bool IsCreated { get; set; }
        public Guid CreatedUserGuid { get; set; }
        public PageData<ChatMessageDto> Messages { get; set; } = PageData<ChatMessageDto>.EmptyPageData();
    }
}
