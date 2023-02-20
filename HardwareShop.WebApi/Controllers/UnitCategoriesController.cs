

using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class UnitCategoriesController : AuthorizedApiControllerBase
    {
        private IUnitCategoryService unitCategoryService;
        public UnitCategoriesController(IUnitCategoryService unitCategoryService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.unitCategoryService = unitCategoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var categories = await unitCategoryService.GetUnitCategoryPageDataAsync(pagingModel, search);
            responseResultBuilder.SetPageData(categories);
            return responseResultBuilder.Build();
        }
    }
}