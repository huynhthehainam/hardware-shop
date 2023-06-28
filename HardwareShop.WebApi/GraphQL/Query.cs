using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.WebApi.GraphQL
{
    public class UnitObjectType
    {
        public string Name { get; set; } = string.Empty;
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OrderableAttribute : Attribute
    {
        public OrderableAttribute(string name)
        {
            Console.WriteLine("Hello" + name);
        }
        public string CheckValue(object value)
        {
            ShopObjectType shop = (ShopObjectType)value;
            return $"Shop: {shop.Name}";
        }
    }

    // For learning
    [Orderable("ABC")]
    [Orderable("CBD")]
    [Serializable]
    public class ShopObjectType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
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
        public async Task<PageData<ShopObjectType>> GetShops([Service] IShopService shopService, [Service] ICurrentUserService currentUserService, PagingModel pagingModel, string? search)
        {
            Console.WriteLine("Before");
            var isAdmin = currentUserService.IsSystemAdmin();
            var shop = new ShopObjectType() { };
            foreach (var attr in Attribute.GetCustomAttributes(shop.GetType(), typeof(OrderableAttribute)))
            {
                if (attr is OrderableAttribute)
                {

                    OrderableAttribute orderableAttr = (OrderableAttribute)attr;
                    Console.WriteLine(orderableAttr.CheckValue(shop));

                }
                else
                {
                    Console.WriteLine("Not Orderable");
                }
            }
            Console.WriteLine("After");
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