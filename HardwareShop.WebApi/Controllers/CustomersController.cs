

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
        private ICustomerService customerService;
        public CustomersController(ICustomerService customerService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.customerService = customerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomersOfCurrentUserShop([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] bool? isInDebt)
        {
            var customers = isInDebt.GetValueOrDefault(false) ? await customerService.GetCustomerInDebtPageDataOfCurrentUserShopAsync(pagingModel, search) : await customerService.GetCustomerPageDataOfCurrentUserShopAsync(pagingModel, search);
            if (customers == null) return responseResultBuilder.Build();
            responseResultBuilder.SetPageData(customers);
            return responseResultBuilder.Build();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerOfCurrentUserShop([FromBody] CreateCustomerCommand command)
        {
            var customer = await customerService.CreateCustomerOfCurrentUserShopAsync(command.Name ?? "", command.Phone, command.Address, command.IsFamiliar, command.PhoneCountryId);
            if (customer == null) return responseResultBuilder.Build();

            responseResultBuilder.SetData(customer);


            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Update")]
        public async Task<IActionResult> UpdateCustomerOfCurrentUserShop([FromRoute] int id, [FromBody] UpdateCustomerCommand command)
        {
            var customer = await customerService.UpdateCustomerOfCurrentUserShopAsync(id, command.Name, command.Phone, command.Address, command.IsFamiliar, command.AmountOfCash);
            if (customer == null)
                return responseResultBuilder.Build();

            responseResultBuilder.SetUpdatedMessage();

            return responseResultBuilder.Build();
        }
    }
}