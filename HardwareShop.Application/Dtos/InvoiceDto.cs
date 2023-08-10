



namespace HardwareShop.Application.Dtos

{
    public class InvoiceDetailDto
    {
        public long Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double TotalCost { get; set; }
        public string? UnitName { get; set; }
        public double OriginalPrice { get; set; }
        public int ProductId { get; set; }
    }
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Code { get; set; }
        public double Deposit { get; set; }
        public double TotalCost { get; set; }
        public double Debt { get; set; }
        public double Rest { get; set; }
        public InvoiceDetailDto[] Details { get; set; } = Array.Empty<InvoiceDetailDto>();
        public string? CustomerPhonePrefix { get; set; }
    }
}