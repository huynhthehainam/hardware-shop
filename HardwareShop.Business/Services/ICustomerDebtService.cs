

using System.Text.Json;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Services
{
    public interface ICustomerDebtService
    {
        Task<CustomerDebtHistory> AddDebtToCustomer(Customer customer, double changeOfDebt, string reason, JsonDocument? reasonParams);
    }
}