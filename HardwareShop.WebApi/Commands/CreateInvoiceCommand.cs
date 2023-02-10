


using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class CreateInvoiceDetailCommand
    {
        [Required]
        public double? OriginalPrice { get; internal set; }
        [Required]
        public int? ProductId { get; internal set; }
        [Required]
        public double? Quantity { get; internal set; }
        [Required]
        public double? Price { get; internal set; }
        public string? Description { get; internal set; }
    }
    public class CreateInvoiceCommand
    {
        [Required]
        public int? CustomerId { get; set; }
        [Required]
        public double? Deposit { get; set; }
        public int? OrderId { get; set; }
        public List<CreateInvoiceDetailCommand> Details { get; set; } = new List<CreateInvoiceDetailCommand>();
    }
}