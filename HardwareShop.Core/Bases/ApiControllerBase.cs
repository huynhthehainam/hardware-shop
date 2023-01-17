using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Bases
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IResponseResultFactory responseResultFactory;
        protected ApiControllerBase(IResponseResultFactory responseResultFactory)
        {
            this.responseResultFactory = responseResultFactory;
        }
    }
}
