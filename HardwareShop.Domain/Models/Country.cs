


using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{
    public class Country : EntityBase
    {
        public Country()
        {
        }

        public Country(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhonePrefix { get; set; } = string.Empty;

        private ICollection<Customer>? customers;
        public ICollection<Customer>? Customers
        {
            get => lazyLoader.Load(this, ref customers);
            set => customers = value;
        }

        private ICollection<User>? users;
        public ICollection<User>? Users
        {
            get => lazyLoader.Load(this, ref users);
            set => users = value;
        }
        private CountryAsset? asset;
        public CountryAsset? Asset
        {
            get => lazyLoader.Load(this, ref asset);
            set => asset = value;

        }
        private ICollection<ShopPhone>? shopPhones;
        public ICollection<ShopPhone>? ShopPhones
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shopPhones) : shopPhones;
            set => shopPhones = value;
        }

    }
}