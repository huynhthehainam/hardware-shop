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

    public class AuthorizedApiController : ApiControllerBase
    {
        public AuthorizedApiController(IResponseResultFactory responseResultFactory) : base(responseResultFactory)
        {
        }
        protected CacheAccountViewModel currentAccount
        {
            get
            {
                var accountClaims = HttpContext.User.Claims;
                return new CacheAccountViewModel { Id = 0 };
            }
        }
    }
}
