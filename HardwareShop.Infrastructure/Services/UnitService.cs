


using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Services
{
    public class UnitService : IUnitService
    {

        private readonly ICurrentUserService currentUserService;
        private readonly DbContext db;
        public UnitService(ICurrentUserService currentUserService, DbContext db)
        {
            this.currentUserService = currentUserService;
            this.db = db;
        }

        public async Task<ApplicationResponse<CreatedUnitDto>> CreateUnitAsync(CreateUnitDto model)
        {
            bool isAdmin = currentUserService.IsSystemAdmin();
            if (!isAdmin)
            {
                return new(ApplicationError.CreateNotPermittedError());
            }
            var unitCategory = await db.Set<UnitCategory>().FirstOrDefaultAsync(e => e.Id == model.UnitCategoryId);
            if (unitCategory == null)
            {
                return new(ApplicationError.CreateInvalidError("UnitCategoryId"));

            }
            CreateIfNotExistResponse<Unit> createIfNotExistResponse = db.CreateIfNotExists(new Unit
            {
                Name = model.Name,
                CompareWithPrimaryUnit = model.CompareWithPrimaryUnit,
                IsPrimary = false,
                UnitCategoryId = unitCategory.Id,
            }, e => new { e.Name, e.UnitCategoryId });
            if (createIfNotExistResponse.IsExist)
            {
                return new(ApplicationError.CreateExistedError("Unit"));

            }
            return new(new CreatedUnitDto
            {
                Id = createIfNotExistResponse.Entity.Id
            });

        }
        public bool IsCashUnitExist(int cashUnitId)
        {
            return db.Set<Unit>().Any(e => e.Id == cashUnitId && e.UnitCategory != null && e.UnitCategory.Name == UnitCategoryConstants.CurrencyCategoryName);
        }

        public async Task<PageData<UnitDto>> GetUnitDtoPageDataAsync(PagingModel pagingModel, string? search, int? categoryId)
        {
            var unitPageData = await db.Set<Unit>().Where(e => categoryId == null || e.UnitCategoryId == categoryId).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Unit>(search, e => new { e.Name })).GetPageDataAsync(pagingModel);

            return unitPageData.ConvertToOtherPageData(e => new UnitDto
            {
                Id = e.Id,
                Name = e.Name,
                UnitCategoryName = e.UnitCategory?.Name,
            });
        }

        public async Task<ApplicationResponse<double>> RoundValue(int unitId, double value)
        {
            Unit? unit = await db.Set<Unit>().Where(e => e.Id == unitId).FirstOrDefaultAsync();
            if (unit == null)
            {
                return new(ApplicationError.CreateNotFoundError("Unit"));
            }
            return new(unit.RoundValue(value));
        }
    }
}