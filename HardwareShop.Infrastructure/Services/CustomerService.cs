using HardwareShop.Application.Dtos;
using HardwareShop.Application.Helpers;
using HardwareShop.Application.Services;
using HardwareShop.Core.Helpers;
using HardwareShop.Domain.Models;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.EntityFrameworkCore;
using HardwareShop.Application.Models;
using HardwareShop.Infrastructure.Extensions;

namespace HardwareShop.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IShopService shopService;

        private readonly ICustomerDebtService customerDebtService;
        private readonly IInvoiceService invoiceService;
        private readonly DbContext db;
        public CustomerService(DbContext db, IInvoiceService invoiceService, ICustomerDebtService customerDebtService, IShopService shopService)
        {
            this.db = db;
            this.shopService = shopService;
            this.invoiceService = invoiceService;
            this.customerDebtService = customerDebtService;
        }



        public async Task<ApplicationResponse<CreatedCustomerDto>> CreateCustomerOfCurrentUserShopAsync(string name, string? phone, string? address, bool isFamiliar, int? phoneCountryId)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));

            }

            var createIfNotExistResponse = db.CreateIfNotExists(new Customer
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
                return new(ApplicationError.CreateExistedError("Customer"));

            }
            return new(new CreatedCustomerDto { Id = createIfNotExistResponse.Entity.Id });

        }

        public async Task<ApplicationResponse<CustomerDto>> UpdateCustomerOfCurrentUserShopAsync(int customerId, string? name, string? phone, string? address, bool? isFamiliar, double? amountOfCash)
        {
            var response = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (response.Result == null) return new() { Error = response.Error };
            var customer = response.Result;
            customer.Name = string.IsNullOrEmpty(name) ? customer.Name : name;
            customer.Phone = string.IsNullOrEmpty(phone) ? customer.Phone : phone;
            customer.Address = string.IsNullOrEmpty(address) ? customer.Address : address;
            customer.IsFamiliar = isFamiliar == null ? customer.IsFamiliar : isFamiliar.Value;
            if (amountOfCash != null && amountOfCash != 0)
            {
                var reason = amountOfCash > 0 ? CustomerDebtHistoryHelper.GenerateDebtReasonWhenBorrowing() : CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingBack();
                await customerDebtService.AddDebtToCustomerAsync(customer, amountOfCash.Value, reason);
            }
            db.Update(customer);
            db.SaveChanges();
            return new(new CustomerDto { Id = customer.Id });
        }

        private async Task<ApplicationResponse<Customer>> GetCustomerOfCurrentUserShopByIdAsync(int customerId)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            var customer = await db.Set<Customer>().FirstOrDefaultAsync(e => e.ShopId == shop.Id && e.Id == customerId);
            if (customer == null)
            {
                return new(ApplicationError.CreateNotFoundError("Customer"));

            }
            return new(customer);
        }

        public async Task<ApplicationResponse<CustomerDto>> GetCustomerDtoOfCurrentUserShopByIdAsync(int customerId)
        {
            var response = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (response.Result == null) return new() { Error = response.Error };
            var customer = response.Result;
            return new(new CustomerDto()
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Debt = customer.Debt?.Amount ?? 0,
                IsFamiliar = customer.IsFamiliar,
                Phone = customer.Phone,
                PhoneCountryId = customer.PhoneCountryId,
                PhonePrefix = customer.PhoneCountry?.PhonePrefix,
            });
        }

        public async Task<ApplicationResponse<PageData<CustomerDebtHistoryDto>>> GetCustomerDebtHistoryDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel)
        {
            var response = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (response.Result == null) return new() { Error = response.Error };
            var customer = response.Result;
            var customerDebtHistoryPageData = await db.Set<CustomerDebtHistory>().Where(e => e.CustomerDebtId == customer.Id).GetPageDataAsync(pagingModel, new OrderQuery<CustomerDebtHistory>[] { new OrderQuery<CustomerDebtHistory>(e => e.CreatedDate, false) });
            return new(customerDebtHistoryPageData.ConvertToOtherPageData(e => new CustomerDebtHistoryDto
            {
                ChangeOfDebt = e.ChangeOfDebt,
                CreatedDate = e.CreatedDate,
                NewDebt = e.NewDebt,
                OldDebt = e.OldDebt,
                Id = e.Id,
                Reason = e.Reason,
                ReasonParams = e.ReasonParams
            }));
        }

        public async Task<ApplicationResponse<PageData<InvoiceDto>>> GetCustomerInvoiceDtoPageDataByCustomerIdAsync(int customerId, PagingModel pagingModel)
        {
            var response = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (response.Result == null) return new() { Error = response.Error };
            var customer = response.Result;
            var invoicePageData = await db.Set<Invoice>().Where(e => e.CustomerId == customer.Id).GetPageDataAsync(pagingModel, new OrderQuery<Invoice>[] { new OrderQuery<Invoice>(e => e.CreatedDate, false) });

            return new(invoicePageData.ConvertToOtherPageData(e => new InvoiceDto
            {
                Id = e.Id,
                Code = e.Code,
                CreatedDate = e.CreatedDate,
                Deposit = e.Deposit,
                TotalCost = e.GetTotalCost(),
            }));

        }

        public async Task<ApplicationResponse> PayAllDebtForCustomerOfCurrentUserShopAsync(int customerId)
        {
            var response = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (response.Result == null) return new() { Error = response.Error };
            var customer = response.Result;
            var debt = customer.Debt?.Amount ?? 0;
            if (debt < 0)
            {
                return new(ApplicationError.CreateInvalidError("Debt"));

            }
            var reason = CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingAll();
            await customerDebtService.AddDebtToCustomerAsync(customer, -debt, reason);
            return new();
        }
        public async Task<ApplicationResponse<byte[]>> GetPdfBytesOfCurrentUserShopCustomerInvoicesAsync(int customerId)
        {
            var response = await GetCustomerOfCurrentUserShopByIdAsync(customerId);
            if (response.Result == null) return new() { Error = response.Error };
            var customer = response.Result;
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
            return new(bytes);
        }

        public async Task<ApplicationResponse<byte[]>> GetAllDebtsPdfAsync()
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }

            var customerPageData = await db.Set<Customer>().Where(e => e.ShopId == shop.Id && e.Debt != null && e.Debt.Amount > 0).
            GetPageDataAsync(new PagingModel(), new OrderQuery<Customer>[] { new OrderQuery<Customer>(e => e.Name, true) });


            var customers = customerPageData.ToArray();
            var rows = new List<string>();
            var rowHtml = System.IO.File.ReadAllText("HtmlTemplates/CustomersDebt/_SingleRow.html");
            var halfIndex = customers.Length / 2;
            var cashUnit = shop.CashUnit;
            for (var i = 0; i < halfIndex + 1; i++)
            {
                var customer = customers[i];
                var informationListString = new List<string>();
                if (!string.IsNullOrEmpty(customer.Name))
                {
                    informationListString.Add(customer.Name);
                }
                if (!string.IsNullOrEmpty(customer.Phone))
                {
                    informationListString.Add($"{customer.PhoneCountry?.PhonePrefix ?? ""}{customer.Phone ?? ""}");
                }
                if (!string.IsNullOrEmpty(customer.Address))
                {
                    informationListString.Add(customer.Address);
                }
                var information = string.Join(" | ", informationListString);
                var row = HtmlHelper.ReplaceKeyWithValue(rowHtml, new Dictionary<string, string>(){
                    {"VALUE_NAME", information},
                    {"VALUE_DEBT",cashUnit == null ?"0" : ( cashUnit.ConvertValueToString(customer.Debt?.Amount ?? 0))}
                });
                rows.Add(row);
            }
            var tableHtml = System.IO.File.ReadAllText("HtmlTemplates/CustomersDebt/_SingleTable.html");
            var rowsStr = string.Join("", rows);
            var table1Str = HtmlHelper.ReplaceKeyWithValue(tableHtml, new Dictionary<string, string>(){
                {"VALUE_ROWS", rowsStr}
            });

            //             table1Str = languageService.Translate(table1Str, new Dictionary<string, Dictionary<SupportedLanguage, string>>(){
            //     {"NAME", new Dictionary<SupportedLanguage, string>(){{
            //         SupportedLanguage.English, "Name"
            //     },{
            //          SupportedLanguage.Vietnamese, "Tên"
            //     }}},
            //      {"DEBT", new Dictionary<SupportedLanguage, string>(){{
            //         SupportedLanguage.English, "Debt"
            //     },{
            //          SupportedLanguage.Vietnamese, "Nợ"
            //     }}}
            // });
            rows.Clear();
            for (var i = halfIndex + 1; i < customers.Length; i++)
            {
                var customer = customers[i];
                var informationListString = new List<string>();
                if (!string.IsNullOrEmpty(customer.Name))
                {
                    informationListString.Add(customer.Name);
                }
                if (!string.IsNullOrEmpty(customer.Phone))
                {
                    informationListString.Add($"{customer.PhoneCountry?.PhonePrefix ?? ""}{customer.Phone ?? ""}");
                }
                if (!string.IsNullOrEmpty(customer.Address))
                {
                    informationListString.Add(customer.Address);
                }
                var information = string.Join(" | ", informationListString);
                var row = HtmlHelper.ReplaceKeyWithValue(rowHtml, new Dictionary<string, string>(){
                    {"VALUE_NAME", information},
                    {"VALUE_DEBT", customer.Debt?.Amount.ToString() ?? "0"}
                });
                rows.Add(row);
            }
            rowsStr = string.Join("", rows);
            var table2Str = HtmlHelper.ReplaceKeyWithValue(tableHtml, new Dictionary<string, string>(){
                {"VALUE_ROWS", rowsStr}
            });

            //             table2Str = languageService.Translate(table2Str, new Dictionary<string, Dictionary<SupportedLanguage, string>>(){
            //     {"NAME", new Dictionary<SupportedLanguage, string>(){{
            //         SupportedLanguage.English, "Name"
            //     },{
            //          SupportedLanguage.Vietnamese, "Tên"
            //     }}},
            //      {"DEBT", new Dictionary<SupportedLanguage, string>(){{
            //         SupportedLanguage.English, "Debt"
            //     },{
            //          SupportedLanguage.Vietnamese, "Nợ"
            //     }}}
            // });

            var debtHtml = System.IO.File.ReadAllText("HtmlTemplates/CustomersDebt/_FullDebt.html");
            var htmlStr = HtmlHelper.ReplaceKeyWithValue(debtHtml, new Dictionary<string, string>(){
                {"VALUE_TABLE_1", table1Str},
                {"VALUE_TABLE_2", table2Str},
            });
            var wrapper = File.ReadAllText("HtmlTemplates/PdfWrapper.html");

            htmlStr = HtmlHelper.ReplaceKeyWithValue(wrapper, new Dictionary<string, string>(){
                {"VALUE_BODY",htmlStr}
            });
            using MemoryStream ms = new();
            ConverterProperties properties = new();
            properties.SetFontProvider(new DefaultFontProvider(true, true, true));
            PdfDocument pdf = new(new PdfWriter(ms));
            Document document = new(pdf, PageSize.A4);
            HtmlConverter.ConvertToPdf(htmlStr, pdf, properties);

            var bytes = ms.ToArray();
            return new(bytes);
        }

        public async Task<ApplicationResponse<PageData<CustomerDto>>> GetCustomerPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            var customerPageData = await db.Set<Customer>().Where(e => e.ShopId == shop.Id).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Customer>(search, e => new
            {
                e.Name,
                e.Address,
                e.Phone
            })).GetPageDataAsync(pagingModel, new OrderQuery<Customer>[] { new OrderQuery<Customer>(e => e.Name, true) });
            return new(customerPageData.ConvertToOtherPageData(e => new CustomerDto
            {
                Id = e.Id,
                Name = e.Name,
                Address = e.Address,
                IsFamiliar = e.IsFamiliar,
                PhonePrefix = e.PhoneCountry?.PhonePrefix,
                PhoneCountryId = e.PhoneCountryId,
                Phone = e.Phone,
                Debt = e.Debt?.Amount ?? 0,
            }));
        }

        public async Task<ApplicationResponse<PageData<CustomerDto>>> GetCustomerInDebtPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            var customerPageData = await db.Set<Customer>().Where(e => e.ShopId == shop.Id && (e.Debt == null || e.Debt.Amount > 0)).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Customer>(search, e => new
            {
                e.Name,
                e.Address,
                e.Phone
            })).GetPageDataAsync(pagingModel, new OrderQuery<Customer>[] { new OrderQuery<Customer>(e => e.Name, true) });
            return new(customerPageData.ConvertToOtherPageData(e => new CustomerDto
            {
                Id = e.Id,
                Name = e.Name,
                Address = e.Address,
                IsFamiliar = e.IsFamiliar,
                PhonePrefix = e.PhoneCountry?.PhonePrefix,
                PhoneCountryId = e.PhoneCountryId,
                Phone = e.Phone,
                Debt = e.Debt?.Amount ?? 0,
            }));
        }
    }
}