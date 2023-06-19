using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.WebApi.GraphQL
{
    public class UnitObjectType
    {
        public string Name { get; set; } = string.Empty;
    }
    public class ShopObjectType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CashUnitId { get; set; }
        // Lazy load cashUnit
        public UnitObjectType? GetCashUnit([Service] DbContext db, [Parent] ShopObjectType shop)
        {
            var unit = db.Set<Unit>().FirstOrDefault(e => e.Id == shop.CashUnitId);
            return unit == null ? null : new UnitObjectType()
            {
                Name = unit.Name
            };
        }
    }
    public sealed class Query
    {

        [Authorize]

        public async Task<PageData<ShopObjectType>> GetShops([Service] IShopService shopService, PagingModel pagingModel, string? search)
        {
            var shopPageData = await shopService.GetShopDtoPageDataAsync(pagingModel, search);

            return shopPageData.ConvertToOtherPageData(e => new ShopObjectType()
            {
                Name = e.Name,
                CashUnitId = e.CashUnitId,
                Id = e.Id
            });
        }
    }
}