

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
        public CustomersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {

        }
        [HttpGet]
        public async Task<IActionResult> GetCustomersOfCurrentUserShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] bool? isInDebt)
        {

            return responseResultBuilder.Build();
        }
        [HttpGet("AllDebtsPdf")]
        public async Task<IActionResult> GetAllDebtsPdf()
        {
            return responseResultBuilder.Build();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerOfCurrentUserShop([FromBody] CreateCustomerCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Update")]
        public async Task<IActionResult> UpdateCustomerOfCurrentUserShop([FromRoute] int id, [FromBody] UpdateCustomerCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/AllInvoicesPdf")]
        public async Task<IActionResult> GetAllInvoicesPdf([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/DebtHistories")]
        public async Task<IActionResult> GetDebtHistoriesOfCustomer([FromRoute] int id, [FromQuery] PagingModel pagingModel)
        {
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Invoices")]
        public async Task<IActionResult> GetInvoicesOfCustomer([FromRoute] int id, [FromQuery] PagingModel pagingModel)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/PayAllDebt")]
        public async Task<IActionResult> PayAllDebt([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }
    }
}