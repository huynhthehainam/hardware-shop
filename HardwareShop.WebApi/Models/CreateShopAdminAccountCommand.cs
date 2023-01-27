using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Models
{
    public class CreateShopAdminAccountCommand
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
