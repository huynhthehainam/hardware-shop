using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
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
        public async Task<IActionResult> GetProducts([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] SortingModel sortingModel)
        {
            var response = await productService.GetProductPageDataOfCurrentUserShopAsync(pagingModel, search, sortingModel);


            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetPageData(result));
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var response = await productService.GetProductDtoOfCurrentUserShopAsync(id);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Thumbnail")]
        public async Task<IActionResult> GetProductThumbnail([FromRoute] int id)
        {
            var response = await productService.GetProductThumbnailAsync(id);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetAsset(result));
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/SelectThumbnail")]
        public async Task<IActionResult> SelectThumbnail([FromRoute] int id, [FromBody] SelectProductThumbnailCommand command)
        {
            var response = await productService.SetProductThumbnailAsync(id, command.AssetId.GetValueOrDefault());
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Assets/{assetId:int}")]
        public async Task<IActionResult> GetProductAssetById([FromRoute] int id, [FromRoute] int assetId)
        {
            var response = await productService.GetProductAssetByIdAsync(id, assetId);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetAsset(result));
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Assets/{assetId:int}/Delete")]
        public async Task<IActionResult> RemoveProductAsset([FromRoute] int id, [FromRoute] int assetId)
        {
            var response = await productService.RemoveProductAssetByIdAsync(id, assetId);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetDeletedMessage());
            return responseResultBuilder.Build();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var response = await productService.CreateProductOfShopAsync(command.Name ?? "", command.UnitId ?? 0,
            command.Mass, command.PricePerMass, command.PercentForFamiliarCustomer, command.PercentForCustomer, command.PriceForFamiliarCustomer, command.PriceForCustomer ?? 0, command.HasAutoCalculatePermission, command.CategoryIds, command.Warehouses?.Select(e => new Tuple<int, double>(e.WarehouseId, e.Quantity)).ToList());
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/UploadImage")]
        public async Task<IActionResult> UploadProductImage([FromRoute] int id, [FromForm] UploadProductImageCommand command)
        {
            if (command.Image == null)
            {
                return responseResultBuilder.Build();
            }
            var response = await productService.UploadProductImageOfCurrentUserShopAsync(id, command.AssetType ?? "", command.Image);

            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetData(result));
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Update")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductCommand command)
        {
            var response = await productService.UpdateProductOfCurrentUserShopAsync(id, command.Name,
                 command.UnitId,
                 command.Mass,
                 command.PricePerMass,
                 command.PercentForFamiliarCustomer,
                 command.PercentForCustomer,
                 command.PriceForFamiliarCustomer,
                 command.PriceForCustomer,
                 command.HasAutoCalculatePermission,
                 command.CategoryIds,
                 command.Warehouses?.Select(e => new Tuple<int, double>(e.WarehouseId, e.Quantity)).ToList()
                 );
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
            return responseResultBuilder.Build();
        }
        [HttpPost("AddPricePerMass")]
        public async Task<IActionResult> AddPricePerMass([FromBody] AddPricePerMassCommand command)
        {
            var response = await productService.AddPricePerMassOfCurrentUserShopAsync(command.CategoryIds ?? new List<int> { }, command.AmountOfCash ?? 0);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/SoftlyDelete")]
        public async Task<IActionResult> SoftyDelete([FromRoute] int id)
        {
            var response = await productService.SoftlyDeleteProductOfCurrentUserShopAsync(id);
            responseResultBuilder.SetApplicationResponse(response, (builder, result) => builder.SetUpdatedMessage());
            return responseResultBuilder.Build();
        }

    }
}
