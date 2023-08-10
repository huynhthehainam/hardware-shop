using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public sealed class ChatSessionAssetConstants
    {
        public const string AvatarType = "avatar";
    }
    public sealed class ChatSession : AssetEntityBase
    {
        public ChatSession()
        {
        }

        public ChatSession(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public string? Name { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public int Id { get; set; }
        public bool IsGroupChat { get; set; }
        private ICollection<ChatSessionMember>? members;
        public ICollection<ChatSessionMember>? Members
        {
            get => lazyLoader?.Load(this, ref members);
            set => members = value;
        }
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