using HardwareShop.Business.Dtos;
using HardwareShop.Business.Helpers;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
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
        private readonly IRepository<Unit> unitRepository;
        private readonly IRepository<CustomerDebtHistory> customerDebtHistoryRepository;
        private readonly IRepository<CustomerDebt> customerDebtRepository;
        public InvoiceService(IRepository<CustomerDebtHistory> customerDebtHistoryRepository, IRepository<CustomerDebt> customerDebtRepository, IShopService shopService, IRepository<Unit> unitRepository, IRepository<Product> productRepository, IRepository<Invoice> invoiceRepository, IResponseResultBuilder responseResultBuilder, IRepository<Customer> customerRepository, IRepository<Order> orderRepository, ICustomerDebtService customerDebtService)
        {
            this.unitRepository = unitRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.customerDebtService = customerDebtService;
            this.customerDebtRepository = customerDebtRepository;
            this.shopService = shopService;
            this.customerRepository = customerRepository;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.customerDebtHistoryRepository = customerDebtHistoryRepository;
            this.invoiceRepository = invoiceRepository;
        }
        public async Task<CreatedInvoiceDto?> CreateInvoiceOfCurrentUserShopAsync(int customerId, double deposit, int? orderId, List<CreateInvoiceDetailDto> details)
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
                    responseResultBuilder.AddInvalidFieldError($"Products[{i}].ProductId");
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
            if (changeOfCash != 0)
            {
                var reason = CustomerDebtHistoryHelper.GenerateDebtReasonWhenBuying(invoice.Code);
                CustomerDebtHistory history = await customerDebtService.AddDebtToCustomerAsync(customer, changeOfCash, reason.Item1, reason.Item2);
                invoice.CurrentDebtHistory = history;
                invoice = await invoiceRepository.UpdateAsync(invoice);
            }
            return new CreatedInvoiceDto { Id = invoice.Id };

        }

        public async Task<InvoiceDto?> GetInvoiceDtoOfCurrentUserShopByIdAsync(int invoiceId)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Staff);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var invoice = await invoiceRepository.GetItemByQueryAsync(e => e.Id == invoiceId && e.ShopId == shop.Id);
            if (invoice == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Invoice");
                return null;
            }

            return new InvoiceDto
            {
                Id = invoice.Id,
                CustomerName = invoice.Customer?.Name,
                CustomerPhone = invoice.Customer?.Phone,
                CustomerAddress = invoice.Customer?.Address,
                CreatedDate = invoice.CreatedDate,
                Code = invoice.Code,
                Deposit = invoice.Deposit,
                TotalCost = invoice.GetTotalCost(),
                Debt = invoice.CurrentDebtHistory?.NewDebt ?? 0,
                Rest = invoice.CurrentDebtHistory?.NewDebt ?? 0,
                Details = (invoice.Details ?? new List<InvoiceDetail>()).Select(e => new InvoiceDetailDto()
                {
                    Id = e.Id,
                    ProductName = e.Product?.Name,
                    Description = e.Description,
                    Quantity = e.Quantity,
                    Price = e.Price,
                    UnitName = e.Product?.Unit?.Name,
                    TotalCost = e.GetTotalCost(),
                }).ToArray(),
            };

        }

        public async Task<PageData<InvoiceDto>?> GetInvoiceDtoPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Staff);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var invoices = await invoiceRepository.GetPageDataByQueryAsync(pagingModel, e => e.ShopId == shop.Id, string.IsNullOrEmpty(search) ? null : new SearchQuery<Invoice>(search, e => new
            {
                e.Code,
            }), new List<QueryOrder<Invoice>>() { new QueryOrder<Invoice>(e => e.CreatedDate, false) });
            return PageData<InvoiceDto>.ConvertFromOtherPageData(invoices, invoice => new InvoiceDto
            {
                Id = invoice.Id,
                CustomerName = invoice.Customer?.Name,
                CustomerPhone = invoice.Customer?.Phone,
                CustomerAddress = invoice.Customer?.Address,
                CreatedDate = invoice.CreatedDate,
                Code = invoice.Code,
                Deposit = invoice.Deposit,
                TotalCost = invoice.GetTotalCost(),
                Debt = invoice.CurrentDebtHistory?.NewDebt ?? 0,
                Rest = invoice.CurrentDebtHistory?.NewDebt ?? 0,
            });
        }

        private async Task<Invoice?> getInvoiceOfCurrentUserShop(int invoiceId)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Staff);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var invoice = await invoiceRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == invoiceId);
            if (invoice == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Invoice");
                return null;
            }
            return invoice;
        }

        public async Task<bool> RestoreInvoiceOfCurrentUserSHopAsync(int id)
        {
            var invoice = await getInvoiceOfCurrentUserShop(id);
            if (invoice == null) return false;
            var customer = invoice.Customer;
            if (customer == null) return false;
            var debtHistory = invoice.CurrentDebtHistory;
            if (debtHistory != null)
            {
                var debt = debtHistory.CustomerDebt;
                if (debt != null)
                {
                    var reason = CustomerDebtHistoryHelper.GenerateDebtReasonWhenRestoringInvoice(invoice.Code);
                    await customerDebtService.AddDebtToCustomerAsync(customer, -debtHistory.ChangeOfDebt, reason.Item1, reason.Item2);
                    return await invoiceRepository.DeleteAsync(invoice);
                }
            }
            return await invoiceRepository.DeleteAsync(invoice);
        }
    }
}