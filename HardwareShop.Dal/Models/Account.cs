using HardwareShop.Core.Bases;
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
    public enum AccountRole
    {
        Admin,
        Staff,
    }
    public sealed class Account : EntityBase
    {
        public int Id { get; set; }
        public String? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public String? HashedPassword { get; set; }
        public AccountRole Role { get; set; } = AccountRole.Staff;
        public Account()
        {

        }
        public Account(ILazyLoader lazyLoader) : base(lazyLoader)
        {
            
        }


        private AccountShop? shopAccount;
        public AccountShop? ShopAccount
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shopAccount) : shopAccount;
            set => shopAccount = value;
        }

    }
}
