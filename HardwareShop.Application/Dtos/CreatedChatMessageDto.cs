namespace HardwareShop.Application.Dtos
{
    public class ChatAffectedUser
    {
        public ChatAffectedUser() { }
        public Guid UserGuid { get; set; }
        public int Unread { get; set; }
    }
    public class CreatedChatMessageDto
    {
        public List<ChatAffectedUser> AffectedUsers { get; set; } = new List<ChatAffectedUser>();
        public Guid CreatedUseGuid { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ChatSessionId { get; set; }
    }
}
