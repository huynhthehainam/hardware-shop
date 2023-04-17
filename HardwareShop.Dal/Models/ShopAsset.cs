﻿using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public static class ShopAssetConstants
    {
        public const string LogoAssetType = "logo";
    }
    public sealed class ShopAsset : EntityBase, IAssetTable
    {
        public ShopAsset()
        {
        }

        public ShopAsset(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string Filename { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public string ContentType { get; set; } = string.Empty;


    }
}
