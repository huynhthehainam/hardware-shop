using HardwareShop.Business.Dtos;
using HardwareShop.Business.Helpers;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Helpers;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;

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

        private readonly IRepository<WarehouseProduct> warehouseProductRepository;
        private readonly ILanguageService languageService;
        public InvoiceService(ILanguageService languageService, IShopService shopService, IRepository<WarehouseProduct> warehouseProductRepository, IRepository<Product> productRepository, IRepository<Invoice> invoiceRepository, IResponseResultBuilder responseResultBuilder, IRepository<Customer> customerRepository, IRepository<Order> orderRepository, ICustomerDebtService customerDebtService)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.customerDebtService = customerDebtService;
            this.shopService = shopService;
            this.customerRepository = customerRepository;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.invoiceRepository = invoiceRepository;
            this.languageService = languageService;
            this.warehouseProductRepository = warehouseProductRepository;
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
                var product = await productRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == detail.ProductId);
                if (product == null)
                {
                    responseResultBuilder.AddInvalidFieldError($"Details[{i}].ProductId");
                    return null;
                }
                if (detail.Quantity > product.InventoryNumber)
                {
                    responseResultBuilder.AddInvalidFieldError($"Details[{i}].Quantity");
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
                CustomerInformation = $"{customer.Name} {customer.Address} {customer.Phone}",
                Details = details.Select(e =>
                {
                    return new InvoiceDetail
                    {
                        Description = e.Description,
                        OriginalPrice = e.OriginalPrice,
                        Price = e.Price,
                        ProductId = e.ProductId,
                        Quantity = e.Quantity,
                    };
                }).ToList(),
            });
            foreach (var detail in invoice.Details ?? Array.Empty<InvoiceDetail>())
            {
                var remainingQuantity = detail.Quantity;
                var warehouseProductPageData = await warehouseProductRepository.GetPageDataByQueryAsync(new PagingModel(), e => e.ProductId == detail.ProductId);
                foreach (var warehouseProduct in warehouseProductPageData.Items)
                {
                    warehouseProduct.Quantity -= Math.Min(remainingQuantity, warehouseProduct.Quantity);
                    remainingQuantity -= Math.Min(remainingQuantity, warehouseProduct.Quantity);
                    await warehouseProductRepository.UpdateAsync(warehouseProduct);
                }
            }

            var totalCost = details.Sum(e => e.Quantity * e.Price);
            var roundedTotalCost = shop.CashUnit == null ? totalCost : shop.CashUnit.RoundValue(totalCost);
            var roundedDeposit = shop.CashUnit == null ? deposit : shop.CashUnit.RoundValue(deposit);
            var changeOfCash = roundedTotalCost - roundedDeposit;
            if (changeOfCash != 0)
            {
                var reason = CustomerDebtHistoryHelper.GenerateDebtReasonWhenBuying(invoice.Code);
                CustomerDebtHistory history = await customerDebtService.AddDebtToCustomerAsync(customer, changeOfCash, reason);
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
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer?.Name,
                CustomerPhone = invoice.Customer?.Phone,
                CustomerAddress = invoice.Customer?.Address,
                CustomerPhonePrefix = invoice.Customer?.PhoneCountry?.PhonePrefix,
                CreatedDate = invoice.CreatedDate,
                Code = invoice.Code,
                Deposit = invoice.Deposit,
                TotalCost = invoice.GetTotalCost(),
                Debt = invoice.CurrentDebtHistory?.OldDebt ?? 0,
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
                    OriginalPrice = e.OriginalPrice,
                    ProductId = e.ProductId,

                }).ToArray(),
            };

        }

        public async Task<PageData<InvoiceDto>?> GetInvoiceDtoPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search, SortingModel sortingModel)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Staff);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var orderQuery = sortingModel.ToOrderQueries<Invoice>();
            orderQuery.AddRange(new List<QueryOrder<Invoice>>() { new QueryOrder<Invoice>(e => e.CreatedDate, false) });
            var invoices = await invoiceRepository.GetPageDataByQueryAsync(pagingModel, e => e.ShopId == shop.Id, string.IsNullOrEmpty(search) ? null : new SearchQuery<Invoice>(search, e => new

            {
                e.Code,
                e.CustomerInformation,
            }), orderQuery);
            return PageData<InvoiceDto>.ConvertFromOtherPageData(invoices, invoice => new InvoiceDto
            {
                Id = invoice.Id,
                CustomerName = invoice.Customer?.Name,
                CustomerPhone = invoice.Customer?.Phone,
                CustomerAddress = invoice.Customer?.Address,
                CreatedDate = invoice.CreatedDate,
                Code = invoice.Code,
                Deposit = invoice.Deposit,
                CustomerId = invoice.CustomerId,
                TotalCost = invoice.GetTotalCost(),
                Debt = invoice.CurrentDebtHistory?.OldDebt ?? 0,
                Rest = invoice.CurrentDebtHistory?.NewDebt ?? 0,
                CustomerPhonePrefix = invoice.Customer?.PhoneCountry?.PhonePrefix,
            });
        }

        private async Task<Invoice?> GetInvoiceOfCurrentUserShopAsync(int invoiceId)
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
            var invoice = await GetInvoiceOfCurrentUserShopAsync(id);
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
                    await customerDebtService.AddDebtToCustomerAsync(customer, -debtHistory.ChangeOfDebt, reason);
                    return await invoiceRepository.DeleteAsync(invoice);
                }
            }
            return await invoiceRepository.DeleteAsync(invoice);
        }
        public string GenerateSingleInvoice(Invoice invoice, bool isAllowedToShowCustomerInformation = true, bool isAllowedToShowCustomerDeposit = true, bool isAllowedToShowShopInformation = true)
        {
            var invoiceHtmlFileName = "HtmlTemplates/Invoice/_SingleInvoice.html";
            var htmlStr = System.IO.File.ReadAllText(invoiceHtmlFileName);
            var cashUnit = invoice.Shop?.CashUnit;
            var shop = invoice.Shop;
            var logo = (invoice.Shop?.Assets ?? new List<ShopAsset>()).FirstOrDefault(e => e.AssetType == ShopAssetConstants.LogoAssetType);
            if (invoice.CurrentDebtHistory != null)
            {
                var oldDebtHtmlStr = System.IO.File.ReadAllText("HtmlTemplates/Invoice/_OldDebt.html");
                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>{
                    {"VALUE_DEBT_STRING", oldDebtHtmlStr}
                });
            }
            else
            {
                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>{
                    {"VALUE_DEBT_STRING", ""}
                });
            }

            if (isAllowedToShowCustomerDeposit)
            {
                var depositHtmlStr = System.IO.File.ReadAllText("HtmlTemplates/Invoice/_Deposit.html");
                depositHtmlStr = languageService.Translate(depositHtmlStr, new Dictionary<string, Dictionary<SupportedLanguage, string>>(){
                    {"DEPOSIT_LABEL", new Dictionary<SupportedLanguage, string>(){
                     { SupportedLanguage.English, "Deposit"},
                   {SupportedLanguage.Vietnamese,  "Trả trước"}
                }},
                {"REST_LABEL", new Dictionary<SupportedLanguage, string>(){
                     { SupportedLanguage.English, "Rest"},
                   {SupportedLanguage.Vietnamese,  "Còn lại"}
                }},
                });
                depositHtmlStr = HtmlHelper.ReplaceKeyWithValue(depositHtmlStr, new Dictionary<string, string>(){
                      {"VALUE_DEPOSIT",cashUnit == null ? "0": cashUnit.ConvertValueToString(invoice.Deposit)},
                {"VALUE_REST", cashUnit == null ? "0": cashUnit.ConvertValueToString(invoice.CurrentDebtHistory?.NewDebt ?? 0)},
                });
                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>(){
                     {"VALUE_DEPOSIT_STRING", depositHtmlStr}
                });
            }
            else
            {
                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>(){
                     {"VALUE_DEPOSIT_STRING", ""}
                });
            }
            if (isAllowedToShowCustomerInformation)
            {
                var customerInformationHtmlStr = System.IO.File.ReadAllText("HtmlTemplates/Invoice/_CustomerInformation.html");
                customerInformationHtmlStr = HtmlHelper.ReplaceKeyWithValue(customerInformationHtmlStr, new Dictionary<string, string>
                {
     {"VALUE_CUSTOMER_PHONE", invoice.Customer?.Name ?? ""},
                {"VALUE_CUSTOMER_ADDRESS", invoice.Customer?.Address ?? ""},
                });

                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>(){
                     {"VALUE_CUSTOMER_INFORMATION_STRING", customerInformationHtmlStr}
                });
            }
            else
            {
                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>(){
                     {"VALUE_CUSTOMER_INFORMATION_STRING", ""}
                });
            }

            if (isAllowedToShowShopInformation && shop != null)
            {
                var shopPhoneHtmlStr = System.IO.File.ReadAllText("HtmlTemplates/Invoice/_ShopPhone.html");
                var phoneListString = new List<string>();
                foreach (var phone in shop.Phones ?? Array.Empty<ShopPhone>())
                {
                    phoneListString.Add(HtmlHelper.ReplaceKeyWithValue(shopPhoneHtmlStr, new Dictionary<string, string>(){
                       { "VALUE_PHONE_PREFIX",phone.Country?.PhonePrefix ?? ""},
                       { "VALUE_PHONE", phone.Phone},
                       { "VALUE_PHONE_OWNER",phone.OwnerName}

                    }));
                }


                var shopInformationHtmlStr = System.IO.File.ReadAllText("HtmlTemplates/Invoice/_ShopInformation.html");
                var imgSrc = logo == null ? "" : logo.ConvertToImgSrc();
                shopInformationHtmlStr = HtmlHelper.ReplaceKeyWithValue(shopInformationHtmlStr, new Dictionary<string, string>() {
                { "VALUE_SHOP_LOGO", imgSrc },
                {"VALUE_SHOP_NAME" , shop.Name   ?? ""},
                     {"VALUE_SHOP_ADDRESS" , shop.Address   ?? ""},
                     {"VALUE_PHONE_STRING",string.Join("", phoneListString) }
            });
                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>(){
                     {"VALUE_SHOP_INFORMATION_STRING", shopInformationHtmlStr}
                });
            }
            else
            {
                htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>(){
                     {"VALUE_SHOP_INFORMATION_STRING", ""}
                });
            }

            htmlStr = languageService.Translate(htmlStr,
            new Dictionary<string, Dictionary<SupportedLanguage, string>>()
            {
                {"INVOICE_LABEL" , new Dictionary<SupportedLanguage, string>()
                {
                   { SupportedLanguage.English, "Invoice"},
                   {SupportedLanguage.Vietnamese, "Hoá đơn"}
                }
                },
                 {
                    "OLD_DEBT_LABEL", new Dictionary<SupportedLanguage, string>{
                        { SupportedLanguage.English, "Old debt"},
                   {SupportedLanguage.Vietnamese, "Nợ trước"}
                    }
                },
                 {"TOTAL_COST_LABEL", new Dictionary<SupportedLanguage, string>(){
                     { SupportedLanguage.English, "Total"},
                   {SupportedLanguage.Vietnamese,  "Thành tiền"}
                }},

                 {"PRODUCT_LABEL", new Dictionary<SupportedLanguage, string>(){
                     { SupportedLanguage.English, "Product"},
                   {SupportedLanguage.Vietnamese,  "Sản phẩm"}
                }},
                  {"PRICE_LABEL", new Dictionary<SupportedLanguage, string>(){
                     { SupportedLanguage.English, "Price"},
                   {SupportedLanguage.Vietnamese,  "Đơn giá"}
                }},
                 {"QUANTITY_LABEL", new Dictionary<SupportedLanguage, string>(){
                     { SupportedLanguage.English, "Quantity"},
                   {SupportedLanguage.Vietnamese,  "Số lượng"}
                }},
            });





            var rowHtmlStr = File.ReadAllText("HtmlTemplates/Invoice/_InvoiceRow.html");
            var rows = new List<string>();

            foreach (var detail in invoice.Details ?? new List<InvoiceDetail>())
            {
                var unit = detail.Product?.Unit;

                var row = HtmlHelper.ReplaceKeyWithValue(rowHtmlStr, new Dictionary<string, string>(){
                    {"VALUE_PRODUCT", detail.Product?.Name ?? ""},
                    {"VALUE_DESCRIPTION", detail.Description ?? ""},
                    {"VALUE_PRICE",cashUnit == null ? "0" : cashUnit.ConvertValueToString(detail.Price)},
                    {"VALUE_UNIT", detail.Product?.Unit?.Name??""},
                    {"VALUE_CASH_UNIT", cashUnit == null ?"": cashUnit.Name},
                    {"VALUE_QUANTITY",unit== null ? "0": unit.ConvertValueToString(detail.Quantity)},
                    {"VALUE_TOTAL_COST",cashUnit == null ? "0": cashUnit.ConvertValueToString(detail.GetTotalCost())},

                });
                rows.Add(row);
            }
            var rowsStr = string.Join("", rows);

            htmlStr = HtmlHelper.ReplaceKeyWithValue(htmlStr, new Dictionary<string, string>(){
                {"VALUE_ROWS", rowsStr},
                {"VALUE_TOTAL_COST",cashUnit == null ? "0": cashUnit.ConvertValueToString(invoice.GetTotalCost())},
                {"VALUE_CASH_UNIT", cashUnit == null ? "":cashUnit.Name},
                {"VALUE_OLD_DEBT",cashUnit == null ? "0": cashUnit.ConvertValueToString(invoice.CurrentDebtHistory?.OldDebt ?? 0)},
                {"VALUE_INVOICE_CODE",$"{invoice.Id:0000}"}
            });
            return htmlStr;
        }
        public async Task<byte[]?> GetPdfBytesOfInvoiceOfCurrentUserShopAsync(int invoiceId, bool isAllowedToShowCustomerInformation, bool isAllowedToShowCustomerDeposit, bool isAllowedToShowShopInformation = true)
        {
            var invoice = await GetInvoiceOfCurrentUserShopAsync(invoiceId);
            if (invoice == null)
            {
                return null;
            }
            var htmlStr = GenerateSingleInvoice(invoice, isAllowedToShowCustomerInformation, isAllowedToShowCustomerDeposit, isAllowedToShowShopInformation);
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
            return bytes;
        }
    }
}