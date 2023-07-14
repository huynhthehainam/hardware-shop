

namespace HardwareShop.Business.Dtos
{
    public sealed class ChatMessageDto
    {
        public long Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}