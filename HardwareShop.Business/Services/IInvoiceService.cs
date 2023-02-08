
using HardwareShop.Business.Dtos;

namespace HardwareShop.Business.Services
{
    public interface IInvoiceService
    {
        Task<CreatedInvoiceDto?> CreateInvoiceAsync(int customerId, double deposit, int? orderId, List<CreateInvoiceDetailDto> details);
    }
}