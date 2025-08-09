using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HotChocolate.Authorization;

namespace HardwareShop.WebApi.GraphQL
{
    public class ShopItemGraphModel : ShopItemDto
    {
        public int GetPhoneCount([Service] ICurrentUserService userService)
        {
            return Phones?.Count() ?? 0;
        }
    }


    public sealed class Query
    {
        [Authorize]
        public async Task<List<ShopItemGraphModel>> GetShops([Service] IShopService shopService, [Service] ICurrentUserService currentUserService, PagingModel pagingModel, string? search)
        {
            Console.WriteLine("Before");
            var isAdmin = currentUserService.IsSystemAdmin();


            var shopPageData = await shopService.GetShopDtoPageDataAsync(pagingModel, search);
            // https://chillicream.com/docs/hotchocolate/v13/get-started-with-graphql-in-net-core
            return shopPageData.Select(item =>
            {
                return new ShopItemGraphModel
                {
                    Address = item.Address,
                    CashUnitId = item.CashUnitId,
                    Name = item.Name,
                    Phones = item.Phones,
                    Emails = item.Emails,
                    Id = item.Id
                };
            }).ToList();
        }
    }
}