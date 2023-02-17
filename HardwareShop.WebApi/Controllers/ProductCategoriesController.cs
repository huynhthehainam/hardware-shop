


using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
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
            var categories = await productCategoryService.GetCategoryPageDataOfCurrentUserShopAsync(pagingModel, search);

            if (categories == null) return responseResultBuilder.Build();

            responseResultBuilder.SetPageData(categories);
            return responseResultBuilder.Build();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var productCategory = await productCategoryService.CreateCategoryOfCurrentUserShopAsync(command.Name ?? "", command.Description);
            if (productCategory == null)
            {
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetData(productCategory);
            return responseResultBuilder.Build();
        }
    }
}