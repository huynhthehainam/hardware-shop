using System.Text.Json;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Enums;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Helpers;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{

    public sealed class User : EntityBase, ISoftDeletable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string? Phone { get; set; }
        public Guid? PhoneCountryId { get; set; }
        public string? SecretValue { get; set; }
        private Country? phoneCountry;
        public Country? PhoneCountry
        {
            get => lazyLoader?.Load(this, ref phoneCountry);
            set => phoneCountry = value;
        }
        public User()
        {

        }
        public User(Action<object, string?> lazyLoader) : base(lazyLoader)
        {

        }
        private UserShop? userShop;
        public UserShop? UserShop
        {
            get => lazyLoader?.Load(this, ref userShop);
            set => userShop = value;
        }
        public bool IsDeleted { get; set; }

        public String InterfaceSettings { get; set; } = InterfaceSettingsHelper.GenerateDefaultInterfaceSettings();

        private ICollection<UserAsset>? assets;
        public ICollection<UserAsset>? Assets
        {
            get => lazyLoader?.Load(this, ref assets);
            set => assets = value;
        }
        private ICollection<Notification>? notifications;
        public ICollection<Notification>? Notifications
        {
            get => lazyLoader?.Load(this, ref notifications);
            set => notifications = value;
        }
        public Guid? GetAvatarAssetId()
        {
            return Assets?.FirstOrDefault(e => e.AssetType == UserAssetConstants.AvatarAssetType)?.AssetId;
        }
    }
}
