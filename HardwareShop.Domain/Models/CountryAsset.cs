using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
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

        public CountryAsset(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int CountryId { get; set; }
        private Country? country;
        public Country? Country
        {
            get => lazyLoader?.Load(this, ref country);
            set => country = value;
        }
    }
}