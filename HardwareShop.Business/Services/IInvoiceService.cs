
using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Business.Services
{
    public interface IInvoiceService
    {
        Task<CreatedInvoiceDto?> CreateInvoiceOfCurrentUserShopAsync(int customerId, double deposit, int? orderId, List<CreateInvoiceDetailDto> details);
        Task<InvoiceDto?> GetInvoiceDtoOfCurrentUserShopByIdAsync(int invoiceId);
        Task<PageData<InvoiceDto>?> GetInvoiceDtoPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
    }
}