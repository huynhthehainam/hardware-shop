using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtService jwtService;
        private const string authPrefix = "Bearer";
        private CacheUser? cacheUser;
        public async Task<CacheUser> GetCacheUserAsync()
        {
            if (cacheUser == null)
            {
                string? authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                authHeader = authHeader.Replace(authPrefix, string.Empty).Trim();

                cacheUser = await jwtService.GetUserFromTokenAsync(authHeader);
            }
            if (cacheUser == null)
            {
                throw new Exception("Token is invalid");
            }
            return cacheUser;
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
