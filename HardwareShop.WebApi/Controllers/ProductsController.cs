using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class ProductsController : AuthorizedApiControllerBase
    {
        private readonly IProductService productService;
        public ProductsController(IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, IProductService productService) : base(responseResultBuilder, currentUserService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] PagingModel pagingModel, [FromQuery] string? search)
        {
            PageData<ProductDto>? productPageData = await productService.GetProductPageDataOfCurrentUserShopAsync(pagingModel, search);
            if (productPageData == null)
            {
                return responseResultBuilder.Build();
            }
            responseResultBuilder.SetPageData(productPageData);
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var product = await productService.GetProductOrCurrentUserShopAsync(id);
            if (product == null) return responseResultBuilder.Build();
            responseResultBuilder.SetData(product);
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Thumbnail")]
        public async Task<IActionResult> GetProductThumbnail([FromRoute] int id)
        {
            var asset = await productService.GetProductThumbnailAsync(id);
            if (asset == null) return responseResultBuilder.Build();

            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Assets/{assetId:int}")]
        public async Task<IActionResult> GetProductAssetById([FromRoute] int id, [FromRoute] int assetId)
        {
            var asset = await productService.GetProductAssetByIdAsync(id, assetId);
            if (asset == null) return responseResultBuilder.Build();
            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            CreatedProductDto? product = await productService.CreateProductOfShopAsync(command.Name ?? "", command.UnitId ?? 0,
            command.Mass, command.PricePerMass, command.PercentForFamiliarCustomer, command.PercentForCustomer, command.PriceForFamiliarCustomer, command.PriceForCustomer ?? 0, command.HasAutoCalculatePermission, command.CategoryIds, command.Warehouses == null ? null : command.Warehouses.Select(e => new Tuple<int, double>(e.WarehouseId, e.Quantity)).ToList());
            if (product == null)
                return responseResultBuilder.Build();

            responseResultBuilder.SetData(product);
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UploadImage")]
        public async Task<IActionResult> UploadProductImage([FromRoute] int id, [FromForm] UploadProductImageCommand command)
        {
            if (command.Image == null)
            {
                return responseResultBuilder.Build();
            }
            var product = await productService.UploadProductImageOfCurrentUserShopAsync(id, command.AssetType ?? "", command.Image);

            if (product == null) return responseResultBuilder.Build();
            responseResultBuilder.SetData(new { Id = product });
            return responseResultBuilder.Build();
        }

    }
}
