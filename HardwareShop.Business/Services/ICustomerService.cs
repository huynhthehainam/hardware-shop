


using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface ICustomerService
    {
        Task<PageData<CustomerDto>?> GetCustomerPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<PageData<CustomerDto>?> GetCustomerInDebtPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<CreatedCustomerDto?> CreateCustomerOfCurrentUserShopAsync(string name, string? phone, string? address, bool isFamiliar);
        Task<CustomerDto?> UpdateCustomerOfCurrentUserShopAsync(int customerId, string? name, string? phone, string? address, bool? isFamiliar, double? amountOfCash);
    }
}