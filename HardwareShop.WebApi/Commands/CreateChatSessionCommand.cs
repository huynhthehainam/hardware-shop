

namespace HardwareShop.WebApi.Commands
{
    public class CreateChatSessionCommand
    {
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
    public class CreateChatMessageCommand
    {
        public int ChatId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}