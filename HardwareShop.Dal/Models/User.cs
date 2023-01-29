using HardwareShop.Core.Bases;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{

    public sealed class User : EntityBase, ISoftDeletable
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
      
        public String? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public String? HashedPassword { get; set; }
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

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(s =>
            {
                s.HasKey(a => a.Id);
                s.HasIndex(e => e.Username).IsUnique();
            });

        }
        public JsonDocument InterfaceSettings { get; set; } = InterfaceSettingsHelper.GenerateDefaultInterfaceSettings();

        private ICollection<UserAsset>? assets;
        public ICollection<UserAsset>? Assets
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref assets) : assets;
            set => assets = value;
        }

    }
}
