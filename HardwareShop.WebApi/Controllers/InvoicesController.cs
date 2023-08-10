

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
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
            var response = await invoiceService.CreateInvoiceOfCurrentUserShopAsync(command.CustomerId.GetValueOrDefault(), command.Deposit.GetValueOrDefault(), command.OrderId,
            command.Details.Select(e => new CreateInvoiceDetailDto
            {
                Description = e.Description,
                OriginalPrice = e.OriginalPrice.GetValueOrDefault(),
                ProductId = e.ProductId.GetValueOrDefault(),
                Quantity = e.Quantity.GetValueOrDefault(),
                Price = e.Price.GetValueOrDefault(),
            }).ToList());
            responseResultBuilder.SetApplicationResponse(response, (builder, invoice) => builder.SetData(invoice));
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceByIdOfCurrentUserShop([FromRoute] int id)
        {
            var response = await invoiceService.GetInvoiceDtoOfCurrentUserShopByIdAsync(id);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
            return responseResultBuilder.Build();
        }

        [HttpPost("{id:int}/Restore")]
        public async Task<IActionResult> RestoreInvoice([FromRoute] int id)
        {
            var response = await invoiceService.RestoreInvoiceOfCurrentUserSHopAsync(id);

            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetDeletedMessage());
            return responseResultBuilder.Build();
        }


        [HttpGet]
        public async Task<IActionResult> GetInvoicesOfCurrentUserShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] SortingModel sortingModel)
        {
            var response = await invoiceService.GetInvoiceDtoPageDataOfCurrentUserShopAsync(pagingModel, search, sortingModel);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetPageData(result));
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/Pdf")]
        public async Task<IActionResult> GetPdf([FromRoute] int id, [FromQuery] bool? isAllowedToShowCustomerInformation, [FromQuery] bool? isAllowedToShowCustomerDeposit, [FromQuery] bool? isAllowedToShowShopInformation)
        {
            var bytes = await invoiceService.GetPdfBytesOfInvoiceOfCurrentUserShopAsync(id, isAllowedToShowCustomerInformation.GetValueOrDefault(false), isAllowedToShowCustomerDeposit.GetValueOrDefault(false), isAllowedToShowShopInformation.GetValueOrDefault(false));
            if (bytes == null) return responseResultBuilder.Build();
            responseResultBuilder.SetFile(bytes, "application/pdf", "invoice.pdf");
            return responseResultBuilder.Build();
        }
    }
}