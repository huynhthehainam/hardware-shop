


using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface ICustomerService
    {
        Task<PageData<CustomerDto>?> GetCustomerPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<PageData<CustomerDto>?> GetCustomerInDebtPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<CreatedCustomerDto?> CreateCustomerOfCurrentUserShopAsync(string name, string? phone, string? address, bool isFamiliar, int? phoneCountryId);
        Task<CustomerDto?> UpdateCustomerOfCurrentUserShopAsync(int customerId, string? name, string? phone, string? address, bool? isFamiliar, double? amountOfCash);
        Task<CustomerDto?> GetCustomerDtoOfCurrentUserShopByIdAsync(int id);
        Task<PageData<CustomerDebtHistoryDto>?> GetCustomerDebtHistoryDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel);

        Task<PageData<InvoiceDto>?> GetCustomerInvoiceDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel);
        Task<bool> PayAllDebtForCustomerOfCurrentUserShopAsync(int id);
        Task<byte[]?> GetPdfBytesOfCurrentUserShopCustomerInvoicesAsync(int customerId);
        Task<byte[]?> GetAllDebtsPdfAsync();
    }
}