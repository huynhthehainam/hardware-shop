

using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Domain.Models
{
    public sealed class ChatSessionMember : EntityBase
    {
        public ChatSessionMember()
        {
        }

        public ChatSessionMember(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        private ChatSession? session;
        public ChatSession? Session
        {
            get => lazyLoader?.Load(this, ref session);
            set => session = value;
        }
        public int SessionId { get; set; }
        private User? user;
        public User? User
        {
            get => lazyLoader?.Load(this, ref user);
            set => user = value;
        }
        public int UserId { get; set; }

        private ICollection<ChatMessage>? messages;
        public ICollection<ChatMessage>? Messages
        {
            get => lazyLoader?.Load(this, ref messages);
            set => messages = value;
        }

        private ICollection<ChatMessageStatus>? messageStatuses;
        public ICollection<ChatMessageStatus>? MessageStatuses
        {
            get => lazyLoader?.Load(this, ref messageStatuses);
            set => messageStatuses = value;
        }
       
    }
}