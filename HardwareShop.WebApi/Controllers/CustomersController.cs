

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class CustomersController : AuthorizedApiControllerBase
    {
        private readonly ICustomerService customerService;
        public CustomersController(ICustomerService customerService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.customerService = customerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomersOfCurrentUserShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] bool? isInDebt)
        {
            var response = isInDebt.GetValueOrDefault(false) ? await customerService.GetCustomerInDebtPageDataOfCurrentUserShopAsync(pagingModel, search) : await customerService.GetCustomerPageDataOfCurrentUserShopAsync(pagingModel, search);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) =>
            {
                builder.SetPageData(result);
            });
            return responseResultBuilder.Build();
        }
        [HttpGet("AllDebtsPdf")]
        public async Task<IActionResult> GetAllDebtsPdf()
        {
            var response = await customerService.GetAllDebtsPdfAsync();

            responseResultBuilder.SetApplicationResponse(response, (builder, bytes) => builder.SetFile(bytes, "application/pdf", "debt.pdf"));
            return responseResultBuilder.Build();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerOfCurrentUserShop([FromBody] CreateCustomerCommand command)
        {
            var response = await customerService.CreateCustomerOfCurrentUserShopAsync(command.Name ?? "", command.Phone, command.Address, command.IsFamiliar, command.PhoneCountryId);


            responseResultBuilder.SetApplicationResponse(response, (builder, customer) => builder.SetData(customer));


            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Update")]
        public async Task<IActionResult> UpdateCustomerOfCurrentUserShop([FromRoute] int id, [FromBody] UpdateCustomerCommand command)
        {
            var response = await customerService.UpdateCustomerOfCurrentUserShopAsync(id, command.Name, command.Phone, command.Address, command.IsFamiliar, command.AmountOfCash);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));

            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById([FromRoute] int id)
        {
            var response = await customerService.GetCustomerDtoOfCurrentUserShopByIdAsync(id);

            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/AllInvoicesPdf")]
        public async Task<IActionResult> GetAllInvoicesPdf([FromRoute] int id)
        {
            var response = await customerService.GetPdfBytesOfCurrentUserShopCustomerInvoicesAsync(id);


            responseResultBuilder.SetApplicationResponse(response, (builder, invoiceBytes) => builder.SetFile(invoiceBytes, "application/pdf", "invoice.pdf"));
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/DebtHistories")]
        public async Task<IActionResult> GetDebtHistoriesOfCustomer([FromRoute] int id, [FromQuery] PagingModel pagingModel)
        {
            var response = await customerService.GetCustomerDebtHistoryDtoPageDataByCustomerIdAsync(id, pagingModel);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetPageData(result));
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Invoices")]
        public async Task<IActionResult> GetInvoicesOfCustomer([FromRoute] int id, [FromQuery] PagingModel pagingModel)
        {
            var response = await customerService.GetCustomerInvoiceDtoPageDataByCustomerIdAsync(id, pagingModel);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetPageData(result));
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/PayAllDebt")]
        public async Task<IActionResult> PayAllDebt([FromRoute] int id)
        {
            var response = await customerService.PayAllDebtForCustomerOfCurrentUserShopAsync(id);
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }
    }
}