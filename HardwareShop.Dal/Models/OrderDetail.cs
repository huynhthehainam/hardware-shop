using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public sealed class OrderDetail : EntityBase
    {
        public OrderDetail()
        {
        }

        public OrderDetail(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public long Id { get; set; }
        public int OrderId { get; set; }
        private Order? order;
        public Order? Order
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref order) : order;
            set => order = value;
        }

        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref product) : product;
            set => product = value;
        }

        public double Quantity { get; set; }
        public string? Description { get; set; }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>(m =>
            {
                m.HasKey(e => e.Id);
                m.HasOne(e => e.Order).WithMany(e => e.Details).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
                m.HasOne(e => e.Product).WithMany(e => e.OrderDetails).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
