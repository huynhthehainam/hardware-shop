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
    public class CurrentAccountService : ICurrentAccountService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtService jwtService;
        private const string authPrefix = "Bearer";
        private CacheAccount? currentAccount;
        public async Task<CacheAccount> GetCacheAccountAsync()
        {
            if (currentAccount == null)
            {
                string? authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                authHeader = authHeader.Replace(authPrefix, string.Empty).Trim();

                currentAccount = await jwtService.GetAccountFromTokenAsync(authHeader);
            }
            if (currentAccount == null)
            {
                throw new Exception("Token is invalid");
            }
            return currentAccount;
        }
        public CurrentAccountService(
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.jwtService = jwtService;
        }
        public bool IsSystemAdmin()
        {
            CacheAccount account = GetCacheAccountAsync().Result;
            return account.Role == AccountRole.Admin;
        }
    }
}
