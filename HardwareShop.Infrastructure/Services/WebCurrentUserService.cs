using HardwareShop.Application.Services;
using HardwareShop.Core.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Infrastructure.Services
{
    public class WebCurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private CacheUser? cacheUser;
        public CacheUser GetCacheUser()
        {
            var userPrincipals = (httpContextAccessor.HttpContext?.User) ?? throw new Exception("Invalid token");
            cacheUser ??= new CacheUser(userPrincipals);
            return cacheUser;
        }
        public WebCurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public bool IsSystemAdmin()
        {
            return GetCacheUser().Role == SystemUserRole.Admin;
        }
        public Guid GetUserGuid()
        {
            return GetCacheUser().Guid;
        }
    }
}
