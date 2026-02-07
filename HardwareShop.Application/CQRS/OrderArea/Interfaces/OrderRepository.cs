namespace HardwareShop.Application.CQRS.OrderArea.Interfaces
{
    public interface IOrderRepository : IRepository<Domain.Models.Order>
    {
    }
}