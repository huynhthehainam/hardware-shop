

using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
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
            PageData<Business.Dtos.CustomerDto>? customers = isInDebt.GetValueOrDefault(false) ? await customerService.GetCustomerInDebtPageDataOfCurrentUserShopAsync(pagingModel, search) : await customerService.GetCustomerPageDataOfCurrentUserShopAsync(pagingModel, search);
            if (customers == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetPageData(customers);
            return responseResultBuilder.Build();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerOfCurrentUserShop([FromBody] CreateCustomerCommand command)
        {
            Business.Dtos.CreatedCustomerDto? customer = await customerService.CreateCustomerOfCurrentUserShopAsync(command.Name ?? "", command.Phone, command.Address, command.IsFamiliar, command.PhoneCountryId);
            if (customer == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetData(customer);


            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Update")]
        public async Task<IActionResult> UpdateCustomerOfCurrentUserShop([FromRoute] int id, [FromBody] UpdateCustomerCommand command)
        {
            Business.Dtos.CustomerDto? customer = await customerService.UpdateCustomerOfCurrentUserShopAsync(id, command.Name, command.Phone, command.Address, command.IsFamiliar, command.AmountOfCash);
            if (customer == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();

            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById([FromRoute] int id)
        {
            Business.Dtos.CustomerDto? customer = await customerService.GetCustomerDtoOfCurrentUserShopByIdAsync(id);

            if (customer == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetData(customer);
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/DebtHistories")]
        public async Task<IActionResult> GetDebtHistoriesOfCustomer([FromRoute] int id, [FromQuery] PagingModel pagingModel)
        {
            PageData<Business.Dtos.CustomerDebtHistoryDto>? histories = await customerService.GetCustomerDebtHistoryDtoPageDataByCustomerIdAsync(id, pagingModel);
            if (histories == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetPageData(histories);
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Invoices")]
        public async Task<IActionResult> GetInvoicesOfCustomer([FromRoute] int id, [FromQuery] PagingModel pagingModel)
        {
            PageData<Business.Dtos.InvoiceDto>? invoices = await customerService.GetCustomerInvoiceDtoPageDataByCustomerIdAsync(id, pagingModel);
            if (invoices == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetPageData(invoices);
            return responseResultBuilder.Build();
        }
    }
}