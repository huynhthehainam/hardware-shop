

namespace HardwareShop.WebApi.Commands
{
    public class CreateChatSessionCommand
    {
        public List<int> UserIds { get; set; } = new List<int>();
    }
    public class CreateChatMessageCommand
    {
        public int ChatId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}