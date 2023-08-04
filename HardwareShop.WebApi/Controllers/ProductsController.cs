using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Models;
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
        public async Task<IActionResult> GetProducts([FromQuery] PagingModel pagingModel, [FromQuery] string? search, [FromQuery] SortingModel sortingModel)
        {
            PageData<ProductDto>? productPageData = await productService.GetProductPageDataOfCurrentUserShopAsync(pagingModel, search, sortingModel);

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
            ProductDto? product = await productService.GetProductDtoOfCurrentUserShopAsync(id);
            if (product == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetData(product);
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Thumbnail")]
        public async Task<IActionResult> GetProductThumbnail([FromRoute] int id)
        {
            CachedAsset? asset = await productService.GetProductThumbnailAsync(id);
            if (asset == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/SelectThumbnail")]
        public async Task<IActionResult> SelectThumbnail([FromRoute] int id, [FromBody] SelectProductThumbnailCommand command)
        {
            bool isSuccess = await productService.SetProductThumbnailAsync(id, command.AssetId.GetValueOrDefault());
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
        [HttpGet("{id:int}/Assets/{assetId:int}")]
        public async Task<IActionResult> GetProductAssetById([FromRoute] int id, [FromRoute] int assetId)
        {
            CachedAsset? asset = await productService.GetProductAssetByIdAsync(id, assetId);
            if (asset == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Assets/{assetId:int}/Delete")]
        public async Task<IActionResult> RemoveProductAsset([FromRoute] int id, [FromRoute] int assetId)
        {
            bool isSuccess = await productService.RemoveProductAssetByIdAsync(id, assetId);
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetDeletedMessage();
            return responseResultBuilder.Build();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            CreatedProductDto? product = await productService.CreateProductOfShopAsync(command.Name ?? "", command.UnitId ?? 0,
            command.Mass, command.PricePerMass, command.PercentForFamiliarCustomer, command.PercentForCustomer, command.PriceForFamiliarCustomer, command.PriceForCustomer ?? 0, command.HasAutoCalculatePermission, command.CategoryIds, command.Warehouses?.Select(e => new Tuple<int, double>(e.WarehouseId, e.Quantity)).ToList());
            if (product == null)
            {
                return responseResultBuilder.Build();
            }

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
            int? product = await productService.UploadProductImageOfCurrentUserShopAsync(id, command.AssetType ?? "", command.Image);

            if (product == null)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetData(new { Id = product });
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/Update")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductCommand command)
        {
            bool isSuccess = await productService.UpdateProductOfCurrentUserShopAsync(id, command.Name,
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
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
        [HttpPost("AddPricePerMass")]
        public async Task<IActionResult> AddPricePerMass([FromBody] AddPricePerMassCommand command)
        {
            bool isSuccess = await productService.AddPricePerMassOfCurrentUserShopAsync(command.CategoryIds ?? new List<int> { }, command.AmountOfCash ?? 0);
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetUpdatedMessage();
            return responseResultBuilder.Build();
        }
        [HttpPost("{id:int}/SoftlyDelete")]
        public async Task<IActionResult> SoftyDelete([FromRoute] int id)
        {
            bool isSuccess = await productService.SoftlyDeleteProductOfCurrentUserShopAsync(id);
            if (!isSuccess)
            {
                return responseResultBuilder.Build();
            }

            responseResultBuilder.SetDeletedMessage();
            return responseResultBuilder.Build();
        }
       
    }
}
