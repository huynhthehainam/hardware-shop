


using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ProductCategoriesController : AuthorizedApiControllerBase
    {
        private readonly IProductCategoryService productCategoryService;
        public ProductCategoriesController(IProductCategoryService productCategoryService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.productCategoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var response = await productCategoryService.GetCategoryPageDataOfCurrentUserShopAsync(pagingModel, search);


            responseResultBuilder.SetApplicationResponse(response, (builder, result) =>
            {
                builder.SetPageData(result);
            });
            return responseResultBuilder.Build();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var response = await productCategoryService.CreateCategoryOfCurrentUserShopAsync(command.Name ?? "", command.Description);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) =>
            {
                builder.SetData(result);
            });
            return responseResultBuilder.Build();
        }
    }
}