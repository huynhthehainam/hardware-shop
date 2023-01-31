using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HardwareShop.Business.Dtos
{
    public class LoginShopDto
    {
        public string Name { get; set; }
        public UserShopRole Role { get; set; }


        public LoginShopDto(string name, UserShopRole role)
        {
            Name = name;
            Role = role;
        }
    }
    public class LoginUserDataDto
    {
        public string DisplayName { get; set; }
        public string? Email { get; set; }
        public JsonDocument Settings { get; set; }
        public string[] Shortcuts { get; set; } = new
            string[] { "calendar", "mail", "contacts" };
        public LoginUserDataDto(string displayName, string? email, JsonDocument settings)
        {
            DisplayName = displayName;
            Email = email;
            Settings = settings;
        }
    }
    public class LoginUserDto
    {
        public SystemUserRole Role { get; set; } = SystemUserRole.Staff;
        public LoginUserDataDto Data { get; set; }
        public LoginShopDto? Shop { get; set; }
        public LoginUserDto(SystemUserRole role, LoginUserDataDto data, LoginShopDto? shop)
        {
            Role = role;
            Data = data;
            Shop = shop;
        }
    }
    public class LoginDto
    {
        public string AccessToken { get; set; }
        public string SessionId { get; set; }
        public LoginUserDto User { get; set; }
        public LoginDto(string accessToken, LoginUserDto user, string sessionId)
        {
            AccessToken = accessToken;
            User = user;
            SessionId = sessionId;
        }
    }
}
