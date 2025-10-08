using System.Text.Json.Serialization;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserShopRole
    {
        Admin,
        Staff,
    }
    public sealed class UserShop : EntityBase
    {
        public UserShop()
        {
        }

        public UserShop(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public UserShopRole Role { get; set; } = UserShopRole.Staff;
        public Guid UserId { get; set; }
        private User? user;
        public User? User
        {
            get => lazyLoader?.Load(this, ref user);
            set => user = value;
        }

        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }


    }
}
