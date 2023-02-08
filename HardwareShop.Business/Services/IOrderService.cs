

using HardwareShop.Business.Dtos;

namespace HardwareShop.Business.Services
{
    public interface IOrderService
    {
        Task<CreatedOrderDto?> CreateOrder();
    }
}