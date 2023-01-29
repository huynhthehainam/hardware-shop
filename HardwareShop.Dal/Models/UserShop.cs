using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
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

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserShop>(e =>
            {
                e.HasKey(e => e.UserId);
                e.HasOne(s => s.User).WithOne(e => e.UserShop).HasForeignKey<UserShop>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(s => s.Shop).WithMany(e => e.UserShops).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
