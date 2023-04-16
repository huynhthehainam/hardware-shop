


using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public class ShopPhone : EntityBase
    {
        public ShopPhone()
        {
        }

        public ShopPhone(ILazyLoader lazyLoader) : base(lazyLoader)
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
        public int CountryId { get; set; }
        private Country? country;
        public Country? Country
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref country) : country;
            set => country = value;
        }
        public string OwnerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ShopPhone>(sp =>
            {
                _ = sp.HasKey(s => s.Id);
                sp.HasOne(e => e.Shop).WithMany(e => e.Phones).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                sp.HasOne(e => e.Country).WithMany(e => e.ShopPhones).HasForeignKey(e => e.CountryId).OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}