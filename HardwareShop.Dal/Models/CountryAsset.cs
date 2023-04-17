

using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public class CountryAssetConstants
    {
        public const string IconType = "icon";
    }
    public class CountryAsset : EntityBase, IAssetTable
    {
        public CountryAsset()
        {
        }

        public CountryAsset(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int CountryId { get; set; }
        private Country? country;
        public Country? Country
        {
            get => lazyLoader.Load(this, ref country);
            set => country = value;
        }
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string Filename { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

    }
}