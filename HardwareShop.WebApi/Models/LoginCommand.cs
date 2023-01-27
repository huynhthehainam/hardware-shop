using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Models
{
    public class LoginCommand
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
