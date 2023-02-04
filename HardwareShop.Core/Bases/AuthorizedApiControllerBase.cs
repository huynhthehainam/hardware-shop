using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Bases
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
