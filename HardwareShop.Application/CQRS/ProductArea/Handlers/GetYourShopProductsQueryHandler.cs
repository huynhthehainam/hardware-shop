using HardwareShop.Application.CQRS.ProductArea.Interfaces;
using HardwareShop.Application.CQRS.ProductArea.Queries;
using HardwareShop.Application.CQRS.UserArea.Interfaces;
using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using MediatR;

namespace HardwareShop.Application.CQRS.ProductArea.Handlers;

public class GetYourShopProductsQueryHandler(
    IProductRepository productRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetYourShopProductsQuery, ApplicationResponse<PageData<YourShopProductDto>>>
{
    public async Task<ApplicationResponse<PageData<YourShopProductDto>>> Handle(GetYourShopProductsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();
        var shop = await userRepository.GetShopByUserIdAsync(currentUserId, cancellationToken);
        if (shop is null)
        {
            return ApplicationResponse<PageData<YourShopProductDto>>.Failure(ApplicationError.CreateNotFoundError("Shop not found for the current user."));
        }

        var products = await productRepository.GetPageDataByShopIdAsync(shop.Id, request.PagingModel, request.Search, cancellationToken);
        var mapped = products.ConvertToOtherPageData(p => new YourShopProductDto
        {
            Id = p.Id,
            Name = p.Name,
            UnitId = p.UnitId,
            UnitName = p.Unit?.Name,
            InventoryNumber = p.InventoryNumber,
            IsDeleted = p.IsDeleted
        });

        return ApplicationResponse<PageData<YourShopProductDto>>.Success(mapped);
    }
}
