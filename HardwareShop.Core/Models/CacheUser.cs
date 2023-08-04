using System.Security.Claims;

namespace HardwareShop.Core.Models
{
    public class CacheUser
    {
        public string Username { get; set; } = string.Empty;
        public SystemUserRole Role { get; set; } = SystemUserRole.Staff;
        public string? Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = String.Empty;
        public Guid Guid { get; set; }
        public CacheUser() { }


        public CacheUser(IEnumerable<Claim> claims)
        {
            Username = claims.FirstOrDefault(e => e.Type == ClaimTypes.Name)?.Value ?? "";
            Email = claims.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value ?? "";
            FirstName = claims.FirstOrDefault(e => e.Type == ClaimTypes.GivenName)?.Value ?? "";
            LastName = claims.FirstOrDefault(e => e.Type == ClaimTypes.Surname)?.Value ?? "";
            Role = Enum.Parse<SystemUserRole>(claims.FirstOrDefault(e => e.Type == ClaimTypes.Role)?.Value ?? "");
            Guid = Guid.Parse(claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value ?? "");
        }
        public CacheUser(ClaimsPrincipal claims) : this(claims.Claims)
        {

        }
    }
}
