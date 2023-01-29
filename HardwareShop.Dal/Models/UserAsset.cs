using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public static class UserAssetConstants
    {
        public const string AvatarAssetType = "avatar";
    }
    public sealed class UserAsset : EntityBase, IAssetTable
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

        public byte[] Bytes { get; set; } = new byte[0];
        public string Filename { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public string ContentType { get; set; }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAsset>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.User).WithMany(e => e.Assets).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
