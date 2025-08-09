

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Commands;
using HotChocolate.Authorization;

namespace HardwareShop.WebApi.GraphQL
{
    public sealed class Mutation
    {

        [Authorize]

        public CreatedShopDto? CreateShop([Service] IShopService shopService, [Service] ICurrentUserService currentUserService, CreateShopCommand command, string str)
        {
            if (!currentUserService.IsSystemAdmin())
            {
                return null;
            }
            var response = shopService.CreateShop(command.Name ?? "", command.Address, command.CashUnitId.GetValueOrDefault());
            return response.Result;
        }
    }
}