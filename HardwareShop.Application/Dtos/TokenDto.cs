using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string SessionId { get; set; }
        public TokenDto(
            string accessToken, string refreshToken, string sessionId)
        {

            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
            this.SessionId = sessionId;
        }
    }
}
