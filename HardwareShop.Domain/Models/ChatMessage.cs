

using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{
    public sealed class ChatMessage : EntityBase
    {
        public ChatMessage()
        {
        }

        public ChatMessage(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public long Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        private ChatSessionMember? member;
        public ChatSessionMember? Member
        {
            get => lazyLoader?.Load(this, ref member);
            set => member = value;
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

        private ICollection<ChatMessageStatus>? messageStatuses;
        public ICollection<ChatMessageStatus>? MessageStatuses
        {
            get => lazyLoader?.Load(this, ref messageStatuses);
            set => messageStatuses = value;
        }
    }
}