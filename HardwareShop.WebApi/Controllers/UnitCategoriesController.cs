

using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class UnitCategoriesController : AuthorizedApiControllerBase
    {
        private readonly IUnitCategoryService unitCategoryService;
        public UnitCategoriesController(IUnitCategoryService unitCategoryService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.unitCategoryService = unitCategoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            var response = await unitCategoryService.GetUnitCategoryPageDataAsync(pagingModel, search);
            responseResultBuilder.SetPageData(response);
            return responseResultBuilder.Build();
        }
    }
}