

using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class UnitCategoryService : IUnitCategoryService
    {
        private readonly IRepository<UnitCategory> unitCategoryRepository;
        public UnitCategoryService(IRepository<UnitCategory> unitCategoryRepository)
        {
            this.unitCategoryRepository = unitCategoryRepository;
        }
        public async Task<PageData<UnitCategoryDto>> GetUnitCategoryPageDataAsync(PagingModel pagingModel, string? search)
        {
            var categories = await unitCategoryRepository.GetPageDataByQueryAsync(pagingModel, e => true, string.IsNullOrEmpty(search) ? null : new SearchQuery<UnitCategory>(search, e => new { e.Name }));
            return PageData<UnitCategoryDto>.ConvertFromOtherPageData(categories, e => new UnitCategoryDto { Id = e.Id, Name = e.Name });
        }
    }
}