

using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.WebApi.Commands;
using HotChocolate.Authorization;

namespace HardwareShop.WebApi.GraphQL
{
    public sealed class Mutation
    {

        [Authorize]

        public async Task<CreatedShopDto?> CreateShop([Service] IShopService shopService, CreateShopCommand command, string str)
        {
            return await shopService.CreateShopAsync(command.Name ?? "", command.Address, command.CashUnitId.GetValueOrDefault());
        }
    }
}