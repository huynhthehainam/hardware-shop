using HardwareShop.Application.Services;
using HardwareShop.Core.Models;

namespace HardwareShop.WebApi.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private CacheUser? cacheUser;
        public CacheUser GetCacheUser()
        {
            var userPrincipals = httpContextAccessor.HttpContext?.User;
            if (userPrincipals == null)
            {
                throw new Exception("Invalid token");
            }
            if (cacheUser == null)
            {
                cacheUser = new CacheUser(userPrincipals);
            }
            return cacheUser;
        }
        public CurrentUserService(
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
