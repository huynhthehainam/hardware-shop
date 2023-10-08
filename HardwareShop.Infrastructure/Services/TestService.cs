

using System.Linq.Expressions;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Services
{
    public class TestService : ITestService
    {
        private readonly DbContext db;
        public TestService(DbContext db)
        {
            this.db = db;
        }
        public async Task<int> TestEntityAsync()
        {
            Expression<Func<User, bool>> query = e => true;
            var countTask = db.Set<User>().CountAsync(query);
            var dataTask = db.Set<User>().Where(query).ToListAsync();

            var count = await countTask;
            var data = await dataTask;

            return count;
        }
    }
}