

using HardwareShop.Application.Extensions;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.Services
{
    public interface IAssetService
    {
        ApplicationResponse<CachedAsset> GetAssetById(long id);
    }
}