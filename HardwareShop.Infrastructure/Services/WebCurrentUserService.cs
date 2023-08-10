using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Enums;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Infrastructure.Services
{
    public class WebCurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private ApplicationUserDto? cacheUser;
        public ApplicationUserDto GetCacheUser()
        {
            var userPrincipals = (httpContextAccessor.HttpContext?.User) ?? throw new Exception("Invalid token");
            cacheUser ??= ApplicationUserDtoHelper.CreateFromClaimsPrincipal(userPrincipals);
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
