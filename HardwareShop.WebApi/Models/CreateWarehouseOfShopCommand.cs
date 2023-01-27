using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Models
{
    public class CreateWarehouseOfShopCommand
    {
        [Required]
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
