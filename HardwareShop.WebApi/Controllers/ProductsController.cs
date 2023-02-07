﻿using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
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
        [HttpGet("{id:int}/Thumbnail")]
        public async Task<IActionResult> GetProductThumbnail([FromRoute] int id)
        {
            var asset = await productService.GetProductThumbnail(id);
            if (asset == null) return responseResultBuilder.Build();

            responseResultBuilder.SetAsset(asset);
            return responseResultBuilder.Build();
        }
    }
}