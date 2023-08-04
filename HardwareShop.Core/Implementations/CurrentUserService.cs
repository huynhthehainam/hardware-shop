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
                cacheUser = new CacheUser(httpContextAccessor.HttpContext.User);
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
        public Guid GetUserGuid()
        {
            CacheUser user = GetCacheUserAsync().Result;
            return user.Guid;
        }
    }
}
