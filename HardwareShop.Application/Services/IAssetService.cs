

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Extensions;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IAssetService
    {
        ApplicationResponse<CachedAssetDto> GetAssetById(long id);
    }
}