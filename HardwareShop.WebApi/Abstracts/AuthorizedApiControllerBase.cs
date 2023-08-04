using HardwareShop.Application.Services;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace HardwareShop.WebApi.Abstracts
{
    [Authorize]
    public abstract class AuthorizedApiControllerBase : ApiControllerBase
    {
        protected readonly ICurrentUserService currentUserService;
        public AuthorizedApiControllerBase(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder)
        {
            this.currentUserService = currentUserService;
        }

    }
}
