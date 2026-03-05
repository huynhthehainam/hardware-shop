using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(MainDatabaseContext context) : base(context)
    {
    }

    public async Task<Shop?> GetShopByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return (await context.Users.FirstOrDefaultAsync(s => s.Id == userId, cancellationToken))?.Shop;
    }
}