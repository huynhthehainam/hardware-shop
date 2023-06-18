using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class CreateShopCommand
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public int? CashUnitId { get; set; }
        public string? Address { get; set; }
    }
}
