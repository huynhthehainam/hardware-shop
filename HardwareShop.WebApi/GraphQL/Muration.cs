

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Commands;
using HotChocolate.Authorization;

namespace HardwareShop.WebApi.GraphQL
{
    public sealed class Mutation
    {

        [Authorize]

        public CreatedShopDto? CreateShop([Service] ICurrentUserService currentUserService, CreateShopCommand command, string str)
        {
            return null;

        }
    }
}