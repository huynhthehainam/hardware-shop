

using System.Text.Json;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface ICustomerDebtService
    {
        Task<CustomerDebtHistory> AddDebtToCustomerAsync(Customer customer, double changeOfDebt, Tuple<string, JsonDocument> reason);
    }
}