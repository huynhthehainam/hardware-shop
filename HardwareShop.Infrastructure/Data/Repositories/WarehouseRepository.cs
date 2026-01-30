using HardwareShop.Application.CQRS.WarehouseArea.Interfaces;
using HardwareShop.Domain.Models;

namespace HardwareShop.Infrastructure.Data.Repositories;

public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(MainDatabaseContext context) : base(context)
    {
    }
}