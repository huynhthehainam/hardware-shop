using HardwareShop.Core.Helpers;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HardwareShop.Core.Implementations
{
    public class JwtConfiguration
    { 
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiredDuration { get; set; } = 120;
    }
    public class JwtService : IJwtService
    {
        private const string appName = "h@rdwareShop";
        private const string jwtSubKey = "sub";
        private const string jwtUsernameKey = "username";
        private const string jwtRoleKey = "role";
        private const int refreshTokenExtendedDuration = 30;
        private readonly IDistributedCache distributedCache;
        private readonly JwtConfiguration jwtConfiguration;
        public JwtService(IOptions<JwtConfiguration> options, IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
            this.jwtConfiguration = options.Value;
        }
        private static string GetCacheKey(string sessionId, int id)
        {
            return $"{appName}-{id}-{sessionId}"; ;
        }
        public async Task<CacheAccount?> GetAccountFromTokenAsync(string token)
        {

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            if (jwtToken == null)
            {
                return null;
            }
            var subClaim = jwtToken.Claims.FirstOrDefault(e => e.Type == jwtSubKey);
            if (subClaim == null)
            {
                return null;
            }
            var issuer = jwtToken.Issuer;
            var accountId = Convert.ToInt32(subClaim.Value);
            var cacheKey = GetCacheKey(issuer, accountId);

            var cacheContent = await distributedCache.GetStringAsync(cacheKey);
            if (cacheContent == null)
            {
                return null;
            }

            var cacheAccount = JsonSerializer.Deserialize<CacheAccount>(cacheContent);
            return cacheAccount;
        }
        public LoginResponse? GenerateTokens(CacheAccount cacheAccount)
        {
            var sessionId = RandomStringHelper.RandomString(10);

            var cacheKey = GetCacheKey(sessionId, cacheAccount.Id);

            distributedCache.SetString(cacheKey, JsonSerializer.Serialize(cacheAccount));
            var claims = new Claim[]
            {
                new Claim(jwtSubKey, cacheAccount.Id.ToString()),
                new Claim(jwtUsernameKey, cacheAccount.Username),
                new Claim(jwtRoleKey, cacheAccount.Role.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(sessionId.ToString(), null, claims, null, DateTime.UtcNow.AddMinutes(jwtConfiguration.ExpiredDuration), creds);
            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(jwtToken);
            var jwtRefreshToken
                = new JwtSecurityToken(sessionId.ToString(), null, claims, null, DateTime.UtcNow.AddMinutes(jwtConfiguration.ExpiredDuration + refreshTokenExtendedDuration), creds);

            var refreshToken = handler.WriteToken(jwtRefreshToken);

            return new LoginResponse { AccessToken = accessToken, RefreshToken = refreshToken };

        }

    }
}
