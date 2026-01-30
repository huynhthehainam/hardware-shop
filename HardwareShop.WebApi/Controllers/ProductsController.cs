using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Extensions;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ProductsController : AuthorizedApiControllerBase
    {

        public ProductsController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] SortingModel sortingModel)
        {
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Thumbnail")]
        public async Task<IActionResult> GetProductThumbnail([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/SelectThumbnail")]
        public async Task<IActionResult> SelectThumbnail([FromRoute] int id, [FromBody] SelectProductThumbnailCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Assets/{assetId:int}")]
        public async Task<IActionResult> GetProductAssetById([FromRoute] int id, [FromRoute] int assetId)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Assets/{assetId:int}/Delete")]
        public async Task<IActionResult> RemoveProductAsset([FromRoute] int id, [FromRoute] int assetId)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UploadImage")]
        public async Task<IActionResult> UploadProductImage([FromRoute] int id, [FromForm] UploadProductImageCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Update")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("AddPricePerMass")]
        public async Task<IActionResult> AddPricePerMass([FromBody] AddPricePerMassCommand command)
        {
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/SoftlyDelete")]
        public async Task<IActionResult> SoftyDelete([FromRoute] int id)
        {
            return responseResultBuilder.Build();
        }

    }
}
