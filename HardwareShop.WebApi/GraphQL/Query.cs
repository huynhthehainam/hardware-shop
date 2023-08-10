using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HotChocolate.Authorization;

namespace HardwareShop.WebApi.GraphQL
{


    public sealed class Query
    {
        [Authorize]
        public async Task<PageData<ShopItemDto>> GetShops([Service] IShopService shopService, [Service] ICurrentUserService currentUserService, PagingModel pagingModel, string? search)
        {
            Console.WriteLine("Before");
            var isAdmin = currentUserService.IsSystemAdmin();


            var shopPageData = await shopService.GetShopDtoPageDataAsync(pagingModel, search);

            return shopPageData;
        }
    }
}