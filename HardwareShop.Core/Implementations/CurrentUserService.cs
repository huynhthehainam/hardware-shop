using System.Text.Json;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Core.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtService jwtService;
        private const string authPrefix = "Bearer";
        private CacheUser? cacheUser;
        public Task<CacheUser> GetCacheUserAsync()
        {
            if (cacheUser == null)
            {
                var userClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(e => e.Type == "UserDetail");
                if (userClaim != null)
                {
                    var valueStr = userClaim.Value;
                    cacheUser = JsonSerializer.Deserialize<CacheUser>(userClaim.Value);
                }
            }
            if (cacheUser == null)
            {
                throw new Exception("Token is invalid");
            }
            return Task.FromResult(cacheUser);
        }
        public CurrentUserService(
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.jwtService = jwtService;
        }
        public bool IsSystemAdmin()
        {
            CacheUser user = GetCacheUserAsync().Result;
            return user.Role == SystemUserRole.Admin;
        }
        public int GetUserId()
        {
            CacheUser user = GetCacheUserAsync().Result;
            return user.Id;
        }
    }
}
