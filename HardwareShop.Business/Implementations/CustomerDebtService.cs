using System.Text.Json;
using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class CustomerDebtService : ICustomerDebtService
    {
        private IRepository<CustomerDebt> customerDebtRepository;
        private IRepository<CustomerDebtHistory> customerDebtHistoryRepository;
        public CustomerDebtService(IRepository<CustomerDebt> customerDebtRepository, IRepository<CustomerDebtHistory> customerDebtHistoryRepository)
        {
            this.customerDebtHistoryRepository = customerDebtHistoryRepository;
            this.customerDebtRepository = customerDebtRepository;
        }

        public async Task<CustomerDebtHistory> AddDebtToCustomerAsync(Customer customer, double changeOfDebt, string reason, JsonDocument? reasonParams)
        {
            CustomerDebt debt = await customerDebtRepository.CreateOrUpdateAsync(new CustomerDebt
            {
                CustomerId = customer.Id,
                Amount = 0,
            }, e => new { e.CustomerId }, e => new { e.CustomerId });
            CustomerDebtHistory history = await customerDebtHistoryRepository.CreateAsync(new CustomerDebtHistory
            {
                ChangeOfDebt = changeOfDebt,
                CustomerDebtId = debt.CustomerId,
                OldDebt = debt.Amount,
                NewDebt = debt.Amount + changeOfDebt,
                Reason = reason,
                CreatedDate = DateTime.UtcNow,
                ReasonParams = reasonParams,
            });

            debt.Amount = history.NewDebt;
            debt = await customerDebtRepository.UpdateAsync(debt);
            return history;
        }
    }
}