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
        public async Task<List<ShopItemGraphModel>> GetShops([Service] ICurrentUserService currentUserService, PagingModel pagingModel, string? search)
        {
            return new List<ShopItemGraphModel>();
        }
    }
}