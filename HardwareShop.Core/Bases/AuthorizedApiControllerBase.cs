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

    public abstract class AuthorizedApiControllerBase : ApiControllerBase
    {
        protected readonly ICurrentAccountService currentAccountService;
        public AuthorizedApiControllerBase(IResponseResultBuilder responseResultBuilder, ICurrentAccountService currentAccountService) : base(responseResultBuilder)
        {
            this.currentAccountService = currentAccountService;
        }

    }
}
