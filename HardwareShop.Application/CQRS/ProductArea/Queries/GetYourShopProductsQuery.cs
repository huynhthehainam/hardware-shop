using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using MediatR;

namespace HardwareShop.Application.CQRS.ProductArea.Queries;

public class GetYourShopProductsQuery : IRequest<ApplicationResponse<PageData<YourShopProductDto>>>
{
    public required PagingModel PagingModel { get; set; }
    public string? Search { get; set; }
}
