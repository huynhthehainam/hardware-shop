using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public CacheUser User { get; set; }
        public LoginResponse(
            CacheUser user, string accessToken, string refreshToken)
        {
            this.User = user;
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
        }
    }
}
