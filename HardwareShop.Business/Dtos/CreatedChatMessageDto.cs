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
        public int UserId { get; set; }
        public int Unread { get; set; }
    }
    public class CreatedChatMessageDto
    {
        public List<ChatAffectedUser> AffectedUsers { get; set; } = new List<ChatAffectedUser>();
        public int CreatedUserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ChatSessionId { get; set; }
    }
}
