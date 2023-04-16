using System.Text.Json;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class CustomerDebtService : ICustomerDebtService
    {
        private readonly IRepository<CustomerDebt> customerDebtRepository;
        private readonly IRepository<CustomerDebtHistory> customerDebtHistoryRepository;
        public CustomerDebtService(IRepository<CustomerDebt> customerDebtRepository, IRepository<CustomerDebtHistory> customerDebtHistoryRepository)
        {
            this.customerDebtHistoryRepository = customerDebtHistoryRepository;
            this.customerDebtRepository = customerDebtRepository;
        }

        public async Task<CustomerDebtHistory> AddDebtToCustomerAsync(Customer customer, double changeOfDebt, Tuple<string, JsonDocument> reason)
        {
            CreateOrUpdateResponse<CustomerDebt> createOrUpdateResponse = await customerDebtRepository.CreateOrUpdateAsync(new CustomerDebt
            {
                CustomerId = customer.Id,
                Amount = 0,
            }, e => new { e.CustomerId }, e => new { e.CustomerId });
            CustomerDebt debt = createOrUpdateResponse.Entity;
            CustomerDebtHistory history = await customerDebtHistoryRepository.CreateAsync(new CustomerDebtHistory
            {
                ChangeOfDebt = changeOfDebt,
                CustomerDebtId = debt.CustomerId,
                OldDebt = debt.Amount,
                NewDebt = debt.Amount + changeOfDebt,
                Reason = reason.Item1,
                CreatedDate = DateTime.UtcNow,
                ReasonParams = reason.Item2,
            });

            debt.Amount = history.NewDebt;
            debt = await customerDebtRepository.UpdateAsync(debt);
            return history;
        }
    }
}