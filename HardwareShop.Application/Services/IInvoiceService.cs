
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IInvoiceService
    {
        Task<ApplicationResponse<CreatedInvoiceDto>> CreateInvoiceOfCurrentUserShopAsync(int customerId, double deposit, int? orderId, List<CreateInvoiceDetailDto> details);
        Task<ApplicationResponse<InvoiceDto>> GetInvoiceDtoOfCurrentUserShopByIdAsync(int invoiceId);
        Task<ApplicationResponse<PageData<InvoiceDto>>> GetInvoiceDtoPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search, SortingModel sortingModel);
        Task<ApplicationResponse> RestoreInvoiceOfCurrentUserSHopAsync(int id);
        Task<byte[]?> GetPdfBytesOfInvoiceOfCurrentUserShopAsync(int invoiceId, bool isAllowedToShowCustomerInformation = false, bool isAllowedToShowCustomerDeposit = false, bool isAllowedToShowShopInformation = true);
        string GenerateSingleInvoice(Invoice invoice, bool isAllowedToShowCustomerInformation = true, bool isAllowedToShowCustomerDeposit = true, bool isAllowedToShowShopInformation = true);
    }
}