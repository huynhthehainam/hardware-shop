

using HardwareShop.Application.Dtos;

namespace HardwareShop.Application.Services
{
    public interface IOrderService
    {
        Task<CreatedOrderDto?> CreateOrder();
    }
}