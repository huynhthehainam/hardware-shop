

using HardwareShop.Application.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
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
            PageData<Application.Dtos.UnitCategoryDto> categories = await unitCategoryService.GetUnitCategoryPageDataAsync(pagingModel, search);
            responseResultBuilder.SetPageData(categories);
            return responseResultBuilder.Build();
        }
    }
}