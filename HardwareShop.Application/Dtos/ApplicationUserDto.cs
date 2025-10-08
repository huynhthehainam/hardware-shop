using System.Security.Claims;
using HardwareShop.Domain.Enums;

namespace HardwareShop.Application.Dtos
{
    public class ApplicationUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string? Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = String.Empty;
        public Guid Guid { get; set; }
        public ApplicationUserDto() { }
    }
}
