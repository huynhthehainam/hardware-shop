using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class UpdateQuantityForProductCommand
    {
        [Required]
        public int? ProductId { get; set; }
        [Required]
        public double? Quantity { get; set; }
    }
}
