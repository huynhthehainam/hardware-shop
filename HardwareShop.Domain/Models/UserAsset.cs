using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public static class UserAssetConstants
    {
        public const string AvatarAssetType = "avatar";
    }
    public sealed class UserAsset : AssetEntityBase
    {
        public UserAsset()
        {
        }

        public UserAsset(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        private User? user;
        public User? User
        {
            get => lazyLoader?.Load(this, ref user);
            set => user = value;
        }
    }
}
