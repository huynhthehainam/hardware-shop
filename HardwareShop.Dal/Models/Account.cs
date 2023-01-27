using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
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

    public sealed class Account : EntityBase, ISoftDeletable
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
        public bool IsDeleted { get; set; }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(s =>
            {
                s.HasKey(a => a.Id);
                s.HasIndex(e => e.Username).IsUnique();
            });

        }

    }
}
