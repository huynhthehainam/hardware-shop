using HardwareShop.Business.Dtos;
using HardwareShop.Business.Helpers;
using HardwareShop.Business.Services;
using HardwareShop.Core.Helpers;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace HardwareShop.Business.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IShopService shopService;
        private readonly IRepository<Customer> customerRepository;
        private readonly IRepository<CustomerDebtHistory> customerDebtHistoryRepository;
        private readonly IRepository<Invoice> invoiceRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly ICustomerDebtService customerDebtService;
        private readonly IInvoiceService invoiceService;
        public CustomerService(IRepository<Invoice> invoiceRepository, IInvoiceService invoiceService, ICustomerDebtService customerDebtService, IRepository<CustomerDebtHistory> customerDebtHistoryRepository, IResponseResultBuilder responseResultBuilder, IShopService shopService, IRepository<Customer> customerRepository)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.customerRepository = customerRepository;
            this.shopService = shopService;
            this.invoiceService = invoiceService;
            this.customerDebtHistoryRepository = customerDebtHistoryRepository;
            this.customerDebtService = customerDebtService;
            this.invoiceRepository = invoiceRepository;
        }
        public async Task<PageData<CustomerDto>?> GetCustomerPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customers = await customerRepository.GetDtoPageDataByQueryAsync<CustomerDto>(pagingModel, e => e.ShopId == shop.Id, e => new CustomerDto
            {
                Id = e.Id,
                Name = e.Name,
                Address = e.Address,
                IsFamiliar = e.IsFamiliar,
                PhonePrefix = e.PhoneCountry?.PhonePrefix,
                PhoneCountryId = e.PhoneCountryId,
                Phone = e.Phone,
                Debt = e.Debt?.Amount ?? 0,
            }, string.IsNullOrEmpty(search) ? null : new SearchQuery<Customer>(search, e => new
            {
                e.Name,
                e.Address,
                e.Phone
            }), new List<QueryOrder<Customer>> { new QueryOrder<Customer>(e => e.Name, true) });
            return customers;
        }
        public async Task<PageData<CustomerDto>?> GetCustomerInDebtPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customers = await customerRepository.GetDtoPageDataByQueryAsync<CustomerDto>(pagingModel, e => e.ShopId == shop.Id && (e.Debt == null || e.Debt.Amount > 0), e => new CustomerDto
            {
                Id = e.Id,
                Name = e.Name,
                Address = e.Address,
                IsFamiliar = e.IsFamiliar,
                PhonePrefix = e.PhoneCountry?.PhonePrefix,
                PhoneCountryId = e.PhoneCountryId,
                Phone = e.Phone,
                Debt = e.Debt?.Amount ?? 0,
            }, string.IsNullOrEmpty(search) ? null : new SearchQuery<Customer>(search, e => new
            {
                e.Name,
                e.Address,
                e.Phone
            }), new List<QueryOrder<Customer>> { new QueryOrder<Customer>(e => e.Name, true) });
            return customers;
        }

        public async Task<CreatedCustomerDto?> CreateCustomerOfCurrentUserShopAsync(string name, string? phone, string? address, bool isFamiliar, int? phoneCountryId)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }

            var createIfNotExistResponse = await customerRepository.CreateIfNotExistsAsync(new Customer
            {
                ShopId = shop.Id,
                Name = name,
                Phone = phone,
                Address = address,
                IsFamiliar = isFamiliar,
                PhoneCountryId = phoneCountryId,
            }, e => new { e.Name, e.Address, e.Phone });
            if (createIfNotExistResponse.IsExist)
            {
                responseResultBuilder.AddExistedEntityError("Customer");
                return null;
            }
            return new CreatedCustomerDto { Id = createIfNotExistResponse.Entity.Id };

        }

        public async Task<CustomerDto?> UpdateCustomerOfCurrentUserShopAsync(int customerId, string? name, string? phone, string? address, bool? isFamiliar, double? amountOfCash)
        {
            var customer = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (customer == null) return null;
            customer.Name = string.IsNullOrEmpty(name) ? customer.Name : name;
            customer.Phone = string.IsNullOrEmpty(phone) ? customer.Phone : phone;
            customer.Address = string.IsNullOrEmpty(address) ? customer.Address : address;
            customer.IsFamiliar = isFamiliar == null ? customer.IsFamiliar : isFamiliar.Value;
            if (amountOfCash != null && amountOfCash != 0)
            {
                var reason = amountOfCash > 0 ? CustomerDebtHistoryHelper.GenerateDebtReasonWhenBorrowing() : CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingBack();
                await customerDebtService.AddDebtToCustomerAsync(customer, amountOfCash.Value, reason);
            }
            customer = await customerRepository.UpdateAsync(customer);
            return new CustomerDto { Id = customer.Id };
        }

        private async Task<Customer?> GetCustomerOfCurrentUserShopByIdAsync(int customerId)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customer = await customerRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == customerId);
            if (customer == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Customer");
                return null;
            }
            return customer;
        }

        public async Task<CustomerDto?> GetCustomerDtoOfCurrentUserShopByIdAsync(int customerId)
        {
            var customer = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (customer == null) return null;

            return new CustomerDto()
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Debt = customer.Debt?.Amount ?? 0,
                IsFamiliar = customer.IsFamiliar,
                Phone = customer.Phone,
                PhoneCountryId = customer.PhoneCountryId,
                PhonePrefix = customer.PhoneCountry?.PhonePrefix,

            };
        }

        public async Task<PageData<CustomerDebtHistoryDto>?> GetCustomerDebtHistoryDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel)
        {
            var customer = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (customer == null) return null;

            return await customerDebtHistoryRepository.GetDtoPageDataByQueryAsync(pagingModel, e => e.CustomerDebtId == customerId, e => new CustomerDebtHistoryDto
            {
                ChangeOfDebt = e.ChangeOfDebt,
                CreatedDate = e.CreatedDate,
                NewDebt = e.NewDebt,
                OldDebt = e.OldDebt,
                Id = e.Id,
                Reason = e.Reason,
                ReasonParams = e.ReasonParams
            }, null, new List<QueryOrder<CustomerDebtHistory>>() { new QueryOrder<CustomerDebtHistory>(e => e.CreatedDate, false) });
        }

        public async Task<PageData<InvoiceDto>?> GetCustomerInvoiceDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel)
        {
            var customer = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (customer == null) return null;
            return await invoiceRepository.GetDtoPageDataByQueryAsync(pagingModel, e => e.CustomerId == customer.Id, e => new InvoiceDto
            {
                Id = e.Id,
                Code = e.Code,
                CreatedDate = e.CreatedDate,
                Deposit = e.Deposit,
                TotalCost = e.GetTotalCost(),
            }, null, new List<QueryOrder<Invoice>> { new QueryOrder<Invoice>(e => e.CreatedDate, false) });
        }

        public async Task<bool> PayAllDebtForCustomerOfCurrentUserShopAsync(int id)
        {
            var customer = await GetCustomerOfCurrentUserShopByIdAsync(id);
            if (customer == null) return false;
            var debt = customer.Debt?.Amount ?? 0;
            if (debt < 0)
            {
                responseResultBuilder.AddInvalidFieldError("Debt");
                return false;
            }
            var reason = CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingAll();
            await customerDebtService.AddDebtToCustomerAsync(customer, -debt, reason);
            return true;
        }
        public async Task<byte[]?> GetPdfBytesOfCurrentUserShopCustomerInvoicesAsync(int customerId)
        {
            var customer = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (customer == null) return null;
            var invoices = customer.Invoices ?? Array.Empty<Invoice>();
            var invoiceContents = new List<string>();
            foreach (var invoice in invoices)
            {
                invoiceContents.Add(invoiceService.GenerateSingleInvoice(invoice));
            }
            var htmlStr = string.Join(",", invoiceContents);
            var wrapper = File.ReadAllText("HtmlTemplates/PdfWrapper.html");
            htmlStr = HtmlHelper.ReplaceKeyWithValue(wrapper, new Dictionary<string, string>(){
                {"VALUE_BODY",htmlStr}
            });
            using MemoryStream ms = new();
            ConverterProperties properties = new();
            properties.SetFontProvider(new DefaultFontProvider(true, true, true));
            PdfDocument pdf = new(new PdfWriter(ms));
            iText.Layout.Document document = new(pdf, PageSize.A4);
            HtmlConverter.ConvertToPdf(htmlStr, pdf, properties);

            var bytes = ms.ToArray();
            return bytes;
        }

        public async Task<byte[]?> GetAllDebtsPdfAsync()
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customerPageData = await customerRepository.GetPageDataByQueryAsync(new PagingModel(), e => e.ShopId == shop.Id, null, new List<QueryOrder<Customer>>() { new QueryOrder<Customer>(e => e.Name, true) });
            var customers = customerPageData.Items;
            return null;
        }
    }
}