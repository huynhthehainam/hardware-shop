

namespace HardwareShop.Business.Dtos
{
    public sealed class ChatMessageDto
    {
        public long Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime Time { get; set; }
    }
}