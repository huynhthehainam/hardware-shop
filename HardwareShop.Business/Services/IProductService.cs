using HardwareShop.Business.Dtos;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Services
{
    public interface IProductService
    {
        Task<PageData<ProductDto>?> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<IAssetTable?> GetProductThumbnail(int productId);
    }
}
