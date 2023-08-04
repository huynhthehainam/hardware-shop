

using HardwareShop.Application.Extensions;
using HardwareShop.Application.Services;
using HardwareShop.Core.Services;
using HardwareShop.Domain.Extensions;
using HardwareShop.WebApi.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class AssetsController : AuthorizedApiControllerBase
    {
        private readonly IAssetService assetService;
        public AssetsController(IAssetService assetService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.assetService = assetService;
        }
        [HttpGet("{id:long}")]
        public IActionResult GetAssetById([FromRoute] long id)
        {
            var assetResponse = assetService.GetAssetById(id);
            responseResultBuilder.SetApplicationResponse(assetResponse, (builder, result) =>
            {
                builder.SetAsset(result);
            });
            return responseResultBuilder.Build();
        }
    }
}