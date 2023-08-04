using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json.Serialization;

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

        public UserShop(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public UserShopRole Role { get; set; } = UserShopRole.Staff;
        public int UserId { get; set; }
        private User? user;
        public User? User
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref user) : user;
            set => user = value;
        }

        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }


    }
}
