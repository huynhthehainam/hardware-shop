using System.Text.Json;
using HardwareShop.Application.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Application.Implementations
{
    public class CustomerDebtService : ICustomerDebtService
    {
        private readonly DbContext db;
        public CustomerDebtService(DbContext db)
        {
            this.db = db;
        }

        public Task<CustomerDebtHistory> AddDebtToCustomerAsync(Customer customer, double changeOfDebt, Tuple<string, JsonDocument> reason)
        {
            CreateOrUpdateResponse<CustomerDebt> createOrUpdateResponse = db.CreateOrUpdate(new CustomerDebt
            {
                CustomerId = customer.Id,
                Amount = 0,
            }, e => new { e.CustomerId }, e => new { e.CustomerId });
            CustomerDebt debt = createOrUpdateResponse.Entity;
            CustomerDebtHistory history = new CustomerDebtHistory
            {
                ChangeOfDebt = changeOfDebt,
                CustomerDebtId = debt.CustomerId,
                OldDebt = debt.Amount,
                NewDebt = debt.Amount + changeOfDebt,
                Reason = reason.Item1,
                CreatedDate = DateTime.UtcNow,
                ReasonParams = reason.Item2,
            };
            db.Set<CustomerDebtHistory>().Add(history);

            debt.Amount = history.NewDebt;
            db.Entry(debt).State = EntityState.Modified;
            db.SaveChanges();
            return Task.FromResult(history);
        }
    }
}