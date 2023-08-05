


using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Services
{
    public class UnitService : IUnitService
    {

        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly ICurrentUserService currentUserService;
        private readonly DbContext db;
        public UnitService(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, DbContext db)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.currentUserService = currentUserService;
            this.db = db;
        }

        public async Task<CreatedUnitDto?> CreateUnitAsync(CreateUnitDto model)
        {
            bool isAdmin = currentUserService.IsSystemAdmin();
            if (!isAdmin)
            {
                responseResultBuilder.AddNotPermittedError();
                return null;
            }
            var unitCategory = await db.Set<UnitCategory>().FirstOrDefaultAsync(e => e.Id == model.UnitCategoryId);
            if (unitCategory == null)
            {
                responseResultBuilder.AddInvalidFieldError("UnitCategoryId");
                return null;
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
                responseResultBuilder.AddExistedEntityError("Unit");
                return null;
            }
            return new CreatedUnitDto
            {
                Id = createIfNotExistResponse.Entity.Id
            };

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

        public async Task<double?> RoundValue(int unitId, double value)
        {
            Unit? unit = await db.Set<Unit>().Where(e => e.Id == unitId).FirstOrDefaultAsync();
            if (unit == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Unit");
                return null;
            }
            return unit.RoundValue(value);
        }
    }
}