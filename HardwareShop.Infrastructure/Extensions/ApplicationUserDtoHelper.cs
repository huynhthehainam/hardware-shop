
using System.Security.Claims;
using HardwareShop.Application.Dtos;
using HardwareShop.Domain.Enums;

namespace HardwareShop.Infrastructure.Extensions
{
    public static class ApplicationUserDtoHelper
    {
        public static ApplicationUserDto CreateFromClaims(IEnumerable<Claim> claims)
        {
            return new ApplicationUserDto
            {
                Username = claims.FirstOrDefault(e => e.Type == ClaimTypes.Name)?.Value ?? "",
                Email = claims.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value ?? "",
                FirstName = claims.FirstOrDefault(e => e.Type == ClaimTypes.GivenName)?.Value ?? "",
                LastName = claims.FirstOrDefault(e => e.Type == ClaimTypes.Surname)?.Value ?? "",
                Role = Enum.Parse<SystemUserRole>(claims.FirstOrDefault(e => e.Type == ClaimTypes.Role)?.Value ?? ""),
                Guid = Guid.Parse(claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value ?? ""),
            };
        }
        public static ApplicationUserDto CreateFromClaimsPrincipal(ClaimsPrincipal claims) => CreateFromClaims(claims.Claims);
    }
}