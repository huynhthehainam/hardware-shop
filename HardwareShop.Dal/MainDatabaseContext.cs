using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
namespace HardwareShop.Dal
{
    public class MainDatabaseContext : DbContext
    {
        public MainDatabaseContext(
            DbContextOptions<MainDatabaseContext> options) : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shop>(s =>
            {
                s.HasKey(x => x.Id);
            });
            modelBuilder.Entity<Account>(s =>
            {
                s.HasKey(a => a.Id);
            });
            modelBuilder.Entity<ShopAccount>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(s => s.Account).WithOne(e => e.ShopAccount).HasForeignKey<ShopAccount>(e => e.Id).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(s => s.Shop).WithMany(e => e.ShopAccounts).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}