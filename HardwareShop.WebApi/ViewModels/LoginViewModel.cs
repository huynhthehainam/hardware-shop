using HardwareShop.Core.Models;
using System.Text.Json;

namespace HardwareShop.WebApi.ViewModels
{
    public class LoginUserDataViewModel
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public JsonDocument Settings { get; set; }
        public string[] Shortcuts { get; set; } = new
            string[] { "calendar", "mail", "contacts" };
        public LoginUserDataViewModel(string displayName, string email, JsonDocument settings)
        {
            DisplayName = displayName;
            Email = email;
            Settings = settings;
        }
    }
    public class LoginUserViewModel
    {
        public SystemUserRole Role { get; set; } = SystemUserRole.Staff;
        public LoginUserDataViewModel Data { get; set; }
        public LoginUserViewModel(SystemUserRole role, LoginUserDataViewModel data)
        {
            Role = role;
            Data = data;
        }
    }
    public class LoginViewModel
    {
        public string AccessToken { get; set; }
        public LoginUserViewModel User { get; set; }
        public LoginViewModel(string accessToken, LoginUserViewModel user)
        {
            AccessToken = accessToken;
            User = user;
        }
    }
}
