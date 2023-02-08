


using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class CreateInvoiceCommand
    {
        [Required]
        public int? CustomerId { get; set; }
    }
}