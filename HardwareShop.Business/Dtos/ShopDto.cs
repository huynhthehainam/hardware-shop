using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Dtos
{
    public class ShopDto
    {
        public int Id { get; set; }
        public UserShopRole UserRole { get; set; } = UserShopRole.Staff;
    }
    public class ShopPhoneDto
    {
        public int Id { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string PhonePrefix { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
    public class ShopItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string[]? Emails { get; set; }
        public ShopPhoneDto[] Phones { get; set; } = Array.Empty<ShopPhoneDto>();
    }
}
