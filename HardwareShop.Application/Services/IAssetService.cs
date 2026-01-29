

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;

namespace HardwareShop.Application.Services
{
    public interface IAssetService
    {
        ApplicationResponse<CachedAssetDto> GetAssetById(Guid id);
    }
}