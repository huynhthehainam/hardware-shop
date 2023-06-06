using HardwareShop.Core.Bases;

namespace HardwareShop.Core.Models
{
    public class OrderQuery<T> where T : EntityBase
    {
        public Func<T, object?> Order { get; set; }
        public bool IsAscending { get; set; }
        public OrderQuery(Func<T, object?> order, bool isAscending)
        {
            Order = order;
            IsAscending = isAscending;
        }
    }
}