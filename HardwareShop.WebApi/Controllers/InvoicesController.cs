

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
        public InvoicesController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceByIdOfCurrentUserShop([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

        [HttpPost("{id:int}/Restore")]
        public async Task<IActionResult> RestoreInvoice([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }


        [HttpGet]
        public async Task<IActionResult> GetInvoicesOfCurrentUserShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] SortingModel sortingModel)
        {
            return responseResultBuilder.Build();
        }

        [HttpGet("{id:int}/Pdf")]
        public async Task<IActionResult> GetPdf([FromRoute] int id, [FromQuery] bool? isAllowedToShowCustomerInformation, [FromQuery] bool? isAllowedToShowCustomerDeposit, [FromQuery] bool? isAllowedToShowShopInformation)
        {
            return responseResultBuilder.Build();
        }
    }
}