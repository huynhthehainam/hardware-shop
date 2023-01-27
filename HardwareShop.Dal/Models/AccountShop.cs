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
    public enum ShopAccountRole
    {
        Admin,
        Staff,
    }
    public sealed class AccountShop : EntityBase
    {
        public AccountShop()
        {
        }

        public AccountShop(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public ShopAccountRole Role { get; set; } = ShopAccountRole.Staff;
        public int AccountId { get; set; }
        private Account? account;
        public Account? Account
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref account) : account;
            set => account = value;
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
            modelBuilder.Entity<AccountShop>(e =>
            {
                e.HasKey(e => e.AccountId);
                e.HasOne(s => s.Account).WithOne(e => e.ShopAccount).HasForeignKey<AccountShop>(e => e.AccountId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(s => s.Shop).WithMany(e => e.ShopAccounts).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
