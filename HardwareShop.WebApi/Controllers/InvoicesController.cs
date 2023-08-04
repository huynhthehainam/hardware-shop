

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class InvoicesController : AuthorizedApiControllerBase
    {
        private readonly IInvoiceService invoiceService;
        public InvoicesController(IInvoiceService invoiceService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
        {
            CreatedInvoiceDto? invoice = await invoiceService.CreateInvoiceOfCurrentUserShopAsync(command.CustomerId.GetValueOrDefault(), command.Deposit.GetValueOrDefault(), command.OrderId,
            command.Details.Select(e => new CreateInvoiceDetailDto
            {
                Description = e.Description,
                OriginalPrice = e.OriginalPrice.GetValueOrDefault(),
                ProductId = e.ProductId.GetValueOrDefault(),
                Quantity = e.Quantity.GetValueOrDefault(),
                Price = e.Price.GetValueOrDefault(),
            }).ToList());
            if (invoice == null) return responseResultBuilder.Build();
            responseResultBuilder.SetData(invoice);
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceByIdOfCurrentUserShop([FromRoute] int id)
        {
            InvoiceDto? invoice = await invoiceService.GetInvoiceDtoOfCurrentUserShopByIdAsync(id);
            if (invoice == null)
            {
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetData(invoice);
            return responseResultBuilder.Build();
        }

        [HttpPost("{id:int}/Restore")]
        public async Task<IActionResult> RestoreInvoice([FromRoute] int id)
        {
            bool isSuccess = await invoiceService.RestoreInvoiceOfCurrentUserSHopAsync(id);
            if (!isSuccess)
                return responseResultBuilder.Build();
            responseResultBuilder.SetDeletedMessage();
            return responseResultBuilder.Build();
        }


        [HttpGet]
        public async Task<IActionResult> GetInvoicesOfCurrentUserShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] SortingModel sortingModel)
        {
            var invoices = await invoiceService.GetInvoiceDtoPageDataOfCurrentUserShopAsync(pagingModel, search, sortingModel);
            if (invoices == null) return responseResultBuilder.Build();

            responseResultBuilder.SetPageData(invoices);
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/Pdf")]
        public async Task<IActionResult> GetPdf([FromRoute] int id, [FromQuery] bool? isAllowedToShowCustomerInformation, [FromQuery] bool? isAllowedToShowCustomerDeposit, [FromQuery] bool? isAllowedToShowShopInformation )
        {
            var bytes = await invoiceService.GetPdfBytesOfInvoiceOfCurrentUserShopAsync(id, isAllowedToShowCustomerInformation.GetValueOrDefault(false), isAllowedToShowCustomerDeposit.GetValueOrDefault(false), isAllowedToShowShopInformation.GetValueOrDefault(false));
            if (bytes == null) return responseResultBuilder.Build();
            responseResultBuilder.SetFile(bytes, "application/pdf", "invoice.pdf");
            return responseResultBuilder.Build();
        }
    }
}