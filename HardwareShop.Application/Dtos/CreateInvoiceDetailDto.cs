

namespace HardwareShop.Application.Dtos
{
    public class CreateInvoiceDetailDto
    {
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public double OriginalPrice { get; set; }
        public double Price { get; set; }
    }
}