using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Dtos
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
