

using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;

namespace HardwareShop.Application.Services
{
    public interface IAssetService
    {
        Task<ApplicationResponse<CachedAssetDto>> GetAssetByIdAsync(Guid id);
    }
}