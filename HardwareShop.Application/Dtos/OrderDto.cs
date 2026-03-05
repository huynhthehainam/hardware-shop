
namespace HardwareShop.Application.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
    }

    public class DetailedOrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalAmount { get; set; }
        public string? Note { get; set; }
    }

    public class DetailedOrderDto
    {
        public Guid Id { get; set; }
        public string? ShopName { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerPhonePrefix { get; set; }
        public string? CustomerAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public double CustomerDebtBeforeOrder { get; set; }
        public double PaidAmount { get; set; }
        public double TotalAmount { get; set; }
        public double DebtAfterOrder { get; set; }
        public DetailedOrderItemDto[] Details { get; set; } = Array.Empty<DetailedOrderItemDto>();
    }
}
