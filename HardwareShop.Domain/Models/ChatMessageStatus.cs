using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public sealed class ChatMessageStatus : EntityBase
    {
        public ChatMessageStatus()
        {
        }

        public ChatMessageStatus(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public bool IsRead { get; set; }

        private ChatSessionMember? member;
        public ChatSessionMember? Member
        {
            get => lazyLoader?.Load(this, ref member);
            set => member = value;
        }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

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

        private ChatMessage? message;
        public ChatMessage? Message
        {
            get => lazyLoader?.Load(this, ref message);
            set => message = value;
        }
        public long MessageId { get; set; }
    }
}