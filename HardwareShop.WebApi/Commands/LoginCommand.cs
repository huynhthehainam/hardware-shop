using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class LoginCommand
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
    public sealed class LoginByTokenCommand
    {
        [Required]
        public string? Token { get; set; }
    }
}
