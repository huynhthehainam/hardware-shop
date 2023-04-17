using System.Text.Json;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{

    public sealed class User : EntityBase, ISoftDeletable
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

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


    }
}
