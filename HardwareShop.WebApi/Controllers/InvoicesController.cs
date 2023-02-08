

using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class InvoicesController : AuthorizedApiControllerBase
    {
        public InvoicesController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
        }

        [HttpPost]
        public Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
        {
            return Task.FromResult(responseResultBuilder.Build());
        }
    }
}