using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
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

        public UserAsset(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        private User? user;
        public User? User
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref user) : user;
            set => user = value;
        }
    }
}
