
using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class CreateCustomerCommand
    {
        [Required]
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsFamiliar { get; set; } = false;
        public int? PhoneCountryId { get; set; }
    }
}