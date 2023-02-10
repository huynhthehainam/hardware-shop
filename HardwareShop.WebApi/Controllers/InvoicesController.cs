

using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
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
            CreatedInvoiceDto? invoice = await invoiceService.CreateInvoiceAsync(command.CustomerId.GetValueOrDefault(), command.Deposit.GetValueOrDefault(), command.OrderId,
            command.Details.Select(e => new CreateInvoiceDetailDto
            {
                Description = e.Description,
                OriginalPrice = e.OriginalPrice.GetValueOrDefault(),
                ProductId = e.ProductId.GetValueOrDefault(),
                Quantity = e.Quantity.GetValueOrDefault(),
                Price = e.Price.GetValueOrDefault()
            }).ToList());
            if (invoice == null) return responseResultBuilder.Build();
            responseResultBuilder.SetData(invoice);
            return responseResultBuilder.Build();
        }
    }
}