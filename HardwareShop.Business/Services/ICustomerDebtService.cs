

using System.Text.Json;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Services
{
    public interface ICustomerDebtService
    {
        Task<CustomerDebtHistory> AddDebtToCustomerAsync(Customer customer, double changeOfDebt, Tuple<string, JsonDocument> reason);
    }
}