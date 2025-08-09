


using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface ICustomerService
    {
        Task<ApplicationResponse<PageData<CustomerDto>>> GetCustomerPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
          Task<ApplicationResponse<PageData<CustomerDto>>> GetCustomerInDebtPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<ApplicationResponse<CreatedCustomerDto>> CreateCustomerOfCurrentUserShopAsync(string name, string? phone, string? address, bool isFamiliar, int? phoneCountryId);
        Task<ApplicationResponse<CustomerDto>> UpdateCustomerOfCurrentUserShopAsync(int customerId, string? name, string? phone, string? address, bool? isFamiliar, double? amountOfCash);
        Task<ApplicationResponse<CustomerDto>> GetCustomerDtoOfCurrentUserShopByIdAsync(int id);
        Task<ApplicationResponse<PageData<CustomerDebtHistoryDto>>> GetCustomerDebtHistoryDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel);

        Task<ApplicationResponse<PageData<InvoiceDto>>> GetCustomerInvoiceDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel);
        Task<ApplicationResponse> PayAllDebtForCustomerOfCurrentUserShopAsync(int id);
        Task<ApplicationResponse<byte[]>> GetPdfBytesOfCurrentUserShopCustomerInvoicesAsync(int customerId);
        Task<ApplicationResponse<byte[]>> GetAllDebtsPdfAsync();
    }
}