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
        Task<CacheUser?> GetUserFromTokenAsync(string token);
        LoginResponse? GenerateTokens(CacheUser cacheUser);
    }
}
