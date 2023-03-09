

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
        public byte[] Bytes { get; set; } = new byte[0];
        public string Filename { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryAsset>(e =>
            {
                e.HasKey(e => e.CountryId);
                e.HasOne(e => e.Country).WithOne(e => e.Asset).HasForeignKey<CountryAsset>(e => e.CountryId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}