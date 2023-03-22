


using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class UnitService : IUnitService
    {
        private readonly IRepository<Unit> unitRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly ICurrentUserService currentUserService;
        public UnitService(IRepository<Unit> unitRepository, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.unitRepository = unitRepository;
            this.currentUserService = currentUserService;
        }

        public async Task<CreatedUnitDto?> CreateUnit(string name, double stepNumber, int unitCategoryId)
        {
            bool isAdmin = currentUserService.IsSystemAdmin();
            if (!isAdmin)
            {
                responseResultBuilder.AddNotPermittedError();
                return null;
            }
            CreateIfNotExistResponse<Unit> createIfNotExistResponse = await unitRepository.CreateIfNotExistsAsync(new Unit { Name = name, StepNumber = stepNumber, UnitCategoryId = unitCategoryId }, e => new { e.Name, e.UnitCategoryId });
            if (createIfNotExistResponse.IsExist)
            {
                responseResultBuilder.AddExistedEntityError("Unit");
                return null;
            }
            return new CreatedUnitDto
            {
                Id = createIfNotExistResponse.Entity.Id
            };

        }

        public async Task<PageData<UnitDto>> GetUnitDtoPageDataAsync(PagingModel pagingModel, string? search, int? categoryId)
        {
            PageData<Unit> units = await unitRepository.GetPageDataByQueryAsync(pagingModel, e => categoryId == null || e.UnitCategoryId == categoryId, string.IsNullOrEmpty(search) ? null : new SearchQuery<Unit>(search, e => new { e.Name }));
            return PageData<UnitDto>.ConvertFromOtherPageData(units, e => new UnitDto
            {
                Id = e.Id,
                Name = e.Name,
                UnitCategoryName = e.UnitCategory?.Name,
            });
        }

        public async Task<double?> RoundValue(int unitId, double value)
        {
            Unit? unit = await unitRepository.GetItemByQueryAsync(e => e.Id == unitId);
            if (unit == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Unit");
                return null;
            }
            return unit.RoundValue(value);
        }
    }
}