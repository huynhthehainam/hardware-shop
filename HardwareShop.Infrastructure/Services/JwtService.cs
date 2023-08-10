using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Helpers;
using HardwareShop.Core.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HardwareShop.Infrastructure.Services
{
    public class JwtConfiguration
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiredDuration { get; set; } = 120;
    }

    public static class JwtServiceConstants
    {
        public const string AppName = "h@rdwareShop";
        public const string SubKey = "sub";
        public const int RefreshTokenExtendedDuration = 30;
    }
    public class JwtService : IJwtService
    {
        private readonly JwtConfiguration jwtConfiguration;
        public JwtService(IOptions<JwtConfiguration> options)
        {
            this.jwtConfiguration = options.Value;
        }

        public ApplicationUserDto? GetUserFromToken(string token)
        {

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            if (jwtToken == null)
            {
                return null;
            }
            var subClaim = jwtToken.Claims.FirstOrDefault(e => e.Type == JwtServiceConstants.SubKey);
            if (subClaim == null)
            {
                return null;
            }
            return ApplicationUserDtoHelper.CreateFromClaims(jwtToken.Claims);
        }
        public TokenDto GenerateTokens(ApplicationUserDto cacheUser)
        {
            var sessionId = RandomStringHelper.RandomString(10);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, cacheUser.Guid.ToString()),
                new Claim(JwtServiceConstants.SubKey, cacheUser.Guid.ToString()),
                new Claim(ClaimTypes.Name, cacheUser.Username),
                new Claim(ClaimTypes.Role, cacheUser.Role.ToString()),
                new Claim(ClaimTypes.GivenName, cacheUser.FirstName),
                new Claim(ClaimTypes.Surname, cacheUser.LastName),
                new Claim(ClaimTypes.Email, cacheUser.Email ?? ""),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(sessionId.ToString(), null, claims, null, DateTime.UtcNow.AddMinutes(jwtConfiguration.ExpiredDuration), creds);
            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(jwtToken);
            var jwtRefreshToken
                = new JwtSecurityToken(sessionId.ToString(), null, claims, null, DateTime.UtcNow.AddMinutes(jwtConfiguration.ExpiredDuration + JwtServiceConstants.RefreshTokenExtendedDuration), creds);

            var refreshToken = handler.WriteToken(jwtRefreshToken);

            return new TokenDto(accessToken, refreshToken, sessionId);

        }


    }
}
