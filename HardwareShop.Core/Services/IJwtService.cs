using HardwareShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Services
{
    public interface IJwtService
    {
        Task<CacheAccount?> GetAccountFromTokenAsync(string token);
        LoginResponse? GenerateTokens(CacheAccount cacheAccount);
    }
}
