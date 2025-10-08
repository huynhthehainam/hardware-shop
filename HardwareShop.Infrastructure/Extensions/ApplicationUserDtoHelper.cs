
using System.Security.Claims;
using HardwareShop.Application.Dtos;
using HardwareShop.Domain.Enums;

namespace HardwareShop.Infrastructure.Extensions
{
    public static class ApplicationUserDtoHelper
    {
        public static ApplicationUserDto CreateFromClaims(IEnumerable<Claim> claims)
        {
            List<string> roles = new();

            // Find role-related claims
            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.Role || claim.Type == "role")
                {
                    roles.Add(claim.Value);
                }
                else if (claim.Type == "realm_access" && !string.IsNullOrEmpty(claim.Value))
                {
                    try
                    {
                        var json = System.Text.Json.JsonDocument.Parse(claim.Value);
                        if (json.RootElement.TryGetProperty("roles", out var roleArray))
                        {
                            roles.AddRange(roleArray.EnumerateArray()
                                .Select(r => r.GetString())
                                .Where(r => !string.IsNullOrEmpty(r))!);
                        }
                    }
                    catch
                    {
                        // Ignore parsing errors
                    }
                }
                else if (claim.Type.StartsWith("resource_access"))
                {
                    try
                    {
                        var json = System.Text.Json.JsonDocument.Parse(claim.Value);
                        foreach (var client in json.RootElement.EnumerateObject())
                        {
                            if (client.Value.TryGetProperty("roles", out var clientRoles))
                            {
                                roles.AddRange(clientRoles.EnumerateArray()
                                    .Select(r => r.GetString())
                                    .Where(r => !string.IsNullOrEmpty(r))!);
                            }
                        }
                    }
                    catch
                    {
                        // Ignore parsing errors
                    }
                }
            }

            return new ApplicationUserDto
            {
                Username = claims.FirstOrDefault(e => e.Type == "preferred_username")?.Value ?? "",
                Email = claims.FirstOrDefault(e => e.Type == "email")?.Value ?? "",
                FirstName = claims.FirstOrDefault(e => e.Type == "given_name")?.Value ?? "",
                LastName = claims.FirstOrDefault(e => e.Type == "family_name")?.Value ?? "",
                Roles = roles.Distinct().ToArray(),
                Guid = Guid.TryParse(claims.FirstOrDefault(e => e.Type == "sub")?.Value ?? "", out var id)
                                ? id
                                : Guid.Empty
            };
        }

        public static ApplicationUserDto CreateFromClaimsPrincipal(ClaimsPrincipal claims) => CreateFromClaims(claims.Claims);
    }
}