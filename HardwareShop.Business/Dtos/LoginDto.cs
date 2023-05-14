using System.Text.Json;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Dtos
{

    public class LoginShopDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserShopRole Role { get; set; }
        public string CashUnitName { get; set; }
        public int CashUnitId { get; set; }
        public ShopPhoneDto[] Phones { get; set; } = Array.Empty<ShopPhoneDto>();
        public string[]? Emails { get; set; }
        public string? Address { get; set; }
        public bool IsAllowedToShowInvoiceDownloadOptions { get; set; } = true;
        public LoginShopDto(int id, string name, UserShopRole role, string cashUnitName, int cashUnitId,
       IEnumerable<ShopPhoneDto> phones, string[]? emails, string? address, bool isAllowedToShowInvoiceDownloadOptions)
        {
            Id = id;
            Name = name;
            Role = role;
            CashUnitName = cashUnitName;
            CashUnitId = cashUnitId;
            Phones = phones.ToArray();
            Emails = emails;
            Address = address;
            IsAllowedToShowInvoiceDownloadOptions = isAllowedToShowInvoiceDownloadOptions;
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
