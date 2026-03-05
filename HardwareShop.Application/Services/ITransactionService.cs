
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
namespace HardwareShop.Application.Services;

public interface ITransactionService
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}