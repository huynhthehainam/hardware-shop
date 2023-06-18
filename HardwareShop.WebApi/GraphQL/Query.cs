

using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HotChocolate.Authorization;

namespace HardwareShop.WebApi.GraphQL
{
    public sealed class Query
    {

        [Authorize]

        public async Task<PageData<ShopItemDto>> GetShops([Service] IShopService shopService, PagingModel pagingModel, string? search)
        {
            return await shopService.GetShopDtoPageDataAsync(pagingModel, search);
        }
    }
}