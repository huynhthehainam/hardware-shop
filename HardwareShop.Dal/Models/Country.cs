


using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
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
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(e =>
            {
                e.HasKey(e => e.Id);
            });
        }
    }
}