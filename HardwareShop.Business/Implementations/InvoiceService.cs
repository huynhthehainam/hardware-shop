using HardwareShop.Business.Dtos;
using HardwareShop.Business.Helpers;
using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IShopService shopService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IRepository<Order> orderRepository;
        private readonly IRepository<Customer> customerRepository;
        private readonly IRepository<Product> productRepository;
        private readonly ICustomerDebtService customerDebtService;
        private readonly IRepository<Invoice> invoiceRepository;
        public InvoiceService(IShopService shopService, IRepository<Product> productRepository, IRepository<Invoice> invoiceRepository, IResponseResultBuilder responseResultBuilder, IRepository<Customer> customerRepository, IRepository<Order> orderRepository, ICustomerDebtService customerDebtService)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.customerDebtService = customerDebtService;
            this.shopService = shopService;
            this.customerRepository = customerRepository;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.invoiceRepository = invoiceRepository;
        }
        public async Task<CreatedInvoiceDto?> CreateInvoiceAsync(int customerId, double deposit, int? orderId, List<CreateInvoiceDetailDto> details)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Staff);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customer = await customerRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == customerId);
            if (customer == null)
            {
                responseResultBuilder.AddInvalidFieldError("CustomerId");
                return null;
            }
            if (orderId != null)
            {
                Order? order = await orderRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == orderId);
                if (order == null)
                {
                    responseResultBuilder.AddInvalidFieldError("OrderId");
                    return null;
                }
            }
            for (int i = 0; i < details.Count; i++)
            {
                var detail = details[i];
                var isProductExist = await productRepository.AnyAsync(e => e.ShopId == shop.Id && e.Id == detail.ProductId);
                if (!isProductExist)
                {
                    responseResultBuilder.AddInvalidFieldError($"Products.{i}.ProductId");
                    return null;
                }
            }
            Invoice invoice = await invoiceRepository.CreateAsync(new Invoice
            {
                CustomerId = customer.Id,
                CreatedDate = DateTime.UtcNow,
                Deposit = deposit,
                OrderId = orderId,
                ShopId = shop.Id,
                Details = details.Select(e =>
                {
                    return new InvoiceDetail
                    {
                        Description = e.Description,
                        OriginalPrice = e.OriginalPrice,
                        Price = e.ProductId,
                        ProductId = e.ProductId,
                        Quantity = e.Quantity,

                    };
                }).ToList(),
            });

            var totalCost = details.Sum(e => e.Quantity * e.Price);
            var roundedTotalCost = shop.CashUnit == null ? totalCost : shop.CashUnit.RoundValue(totalCost);
            var roundedDeposit = shop.CashUnit == null ? deposit : shop.CashUnit.RoundValue(deposit);
            var changeOfCash = roundedTotalCost - roundedDeposit;
            if (changeOfCash > 0)
            {
                var reason = CustomerDebtHistoryHelper.GenerateDebtReasonWhenBuying(invoice.Code);
                CustomerDebtHistory history = await customerDebtService.AddDebtToCustomer(customer, changeOfCash, reason.Item1, reason.Item2);
                invoice.CurrentDebtHistory = history;
                invoice = await invoiceRepository.UpdateAsync(invoice);
            }
            return new CreatedInvoiceDto { Id = invoice.Id };

        }
    }
}