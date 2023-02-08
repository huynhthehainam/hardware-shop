using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Dtos
{
    public class ShopDto
    {
        public int Id { get; set; }
        public UserShopRole UserRole { get; set; } = UserShopRole.Staff;
    }
}
