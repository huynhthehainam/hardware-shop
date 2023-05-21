using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public class CountryAssetConstants
    {
        public const string IconType = "icon";
    }
    public class CountryAsset : AssetEntityBase
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
    }
}