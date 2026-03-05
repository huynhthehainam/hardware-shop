using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace HardwareShop.WebApi.Services;

public sealed class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
        {
            return Task.FromResult(principal);
        }

        var existingRoles = new HashSet<string>(
            identity.FindAll(identity.RoleClaimType).Select(c => c.Value),
            StringComparer.OrdinalIgnoreCase);

        foreach (var role in ExtractRoles(identity))
        {
            if (existingRoles.Add(role))
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, role));
            }
        }

        return Task.FromResult(principal);
    }

    private static IEnumerable<string> ExtractRoles(ClaimsIdentity identity)
    {
        // Snapshot claims to avoid modifying the identity while enumerating it.
        var claims = identity.Claims.ToList();
        foreach (var claim in claims)
        {
            if ((claim.Type == ClaimTypes.Role || claim.Type == "role") && !string.IsNullOrWhiteSpace(claim.Value))
            {
                yield return claim.Value;
                continue;
            }

            if (claim.Type == "realm_access")
            {
                foreach (var role in ExtractRolesFromJsonClaim(claim.Value))
                {
                    yield return role;
                }
            }
        }
    }

    private static IEnumerable<string> ExtractRolesFromJsonClaim(string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimValue))
        {
            return Enumerable.Empty<string>();
        }

        List<string> roles = new();
        try
        {
            using var document = JsonDocument.Parse(claimValue);
            if (document.RootElement.TryGetProperty("roles", out var roleArray))
            {
                foreach (var roleElement in roleArray.EnumerateArray())
                {
                    var role = roleElement.GetString();
                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        roles.Add(role!);
                    }
                }
            }
        }
        catch (JsonException)
        {
            return Enumerable.Empty<string>();
        }

        return roles;
    }
}
