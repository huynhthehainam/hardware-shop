

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Application.Implementations
{
    public class UnitCategoryService : IUnitCategoryService
    {
        private readonly DbContext db;
        public UnitCategoryService(DbContext db)
        {
            this.db = db;
        }
        public async Task<PageData<UnitCategoryDto>> GetUnitCategoryPageDataAsync(PagingModel pagingModel, string? search)
        {
            var categoryPageData = await db.Set<UnitCategory>().Where(e => true).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<UnitCategory>(search, e => new { e.Name })).GetPageDataAsync(pagingModel);
            return categoryPageData.ConvertToOtherPageData(e => new UnitCategoryDto { Id = e.Id, Name = e.Name });
           
        }
    }
}