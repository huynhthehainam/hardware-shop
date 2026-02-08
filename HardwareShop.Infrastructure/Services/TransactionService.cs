using System.Data;
using HardwareShop.Application.Services;
using HardwareShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace HardwareShop.Infrastructure.Services;

public class TransactionService(MainDatabaseContext context) : ITransactionService
{


    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }
}