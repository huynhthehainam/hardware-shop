using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.Core.Bases
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IResponseResultBuilder responseResultBuilder;
        protected ApiControllerBase(IResponseResultBuilder responseResultBuilder)
        {
            this.responseResultBuilder = responseResultBuilder;
        }
    }
}
