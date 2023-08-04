using System.Text.Json;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{

    public sealed class User : EntityBase, ISoftDeletable
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? PhoneCountryId { get; set; }
        private Country? phoneCountry;
        public Country? PhoneCountry
        {
            get => lazyLoader.Load(this, ref phoneCountry);
            set => phoneCountry = value;
        }
        public string HashedPassword { get; set; } = string.Empty;
        public SystemUserRole Role { get; set; } = SystemUserRole.Staff;
        public User()
        {

        }
        public User(ILazyLoader lazyLoader) : base(lazyLoader)
        {

        }
        private UserShop? userShop;
        public UserShop? UserShop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref userShop) : userShop;
            set => userShop = value;
        }
        public bool IsDeleted { get; set; }

        public JsonDocument InterfaceSettings { get; set; } = InterfaceSettingsHelper.GenerateDefaultInterfaceSettings();

        private ICollection<UserAsset>? assets;
        public ICollection<UserAsset>? Assets
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref assets) : assets;
            set => assets = value;
        }
        private ICollection<Notification>? notifications;
        public ICollection<Notification>? Notifications
        {
            get => lazyLoader.Load(this, ref notifications);
            set => notifications = value;
        }


        private ICollection<ChatSessionMember>? chatSessionMembers;
        public ICollection<ChatSessionMember>? ChatSessionMembers
        {
            get => lazyLoader?.Load(this, ref chatSessionMembers);
            set => chatSessionMembers = value;
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

        public long? GetAvatarAssetId()
        {
            return Assets?.FirstOrDefault(e => e.AssetType == UserAssetConstants.AvatarAssetType)?.AssetId;
        }
        public Guid Guid { get; set; } = Guid.NewGuid();
    }
}
