

using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Extensions;
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
            var asset = assetService.GetAssetById(id);
            if (asset == null) return responseResultBuilder.Build();
            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
    }
}