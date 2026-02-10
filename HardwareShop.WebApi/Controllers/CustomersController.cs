

using HardwareShop.Application.CQRS.CustomerArea.Commands;
using HardwareShop.Application.CQRS.CustomerArea.Queries;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class CustomersController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, IMediator mediator) : AuthorizedApiControllerBase(responseResultBuilder, currentUserService)
    {

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
            var response = await mediator.Send(command);
            responseResultBuilder.SetApplicationResponse(response);
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCustomerById([FromRoute] Guid id)
        {
            var query = new GetCustomerByIdQuery()
            {
                Id = id
            };
            var response = await mediator.Send(query);
            responseResultBuilder.SetApplicationResponse(response);
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