using HardwareShop.Application.CQRS.OrderArea.Interfaces;
using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Domain.Models;

namespace HardwareShop.Infrastructure.Data.Repositories;
public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(MainDatabaseContext context) : base(context)
    {
    }
}