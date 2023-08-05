
using HardwareShop.Application.Dtos;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IInvoiceService
    {
        Task<CreatedInvoiceDto?> CreateInvoiceOfCurrentUserShopAsync(int customerId, double deposit, int? orderId, List<CreateInvoiceDetailDto> details);
        Task<InvoiceDto?> GetInvoiceDtoOfCurrentUserShopByIdAsync(int invoiceId);
        Task<PageData<InvoiceDto>?> GetInvoiceDtoPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search, SortingModel sortingModel);
        Task<bool> RestoreInvoiceOfCurrentUserSHopAsync(int id);
        Task<byte[]?> GetPdfBytesOfInvoiceOfCurrentUserShopAsync(int invoiceId, bool isAllowedToShowCustomerInformation = false, bool isAllowedToShowCustomerDeposit = false, bool isAllowedToShowShopInformation = true);
        string GenerateSingleInvoice(Invoice invoice, bool isAllowedToShowCustomerInformation = true, bool isAllowedToShowCustomerDeposit = true, bool isAllowedToShowShopInformation = true);
    }
}