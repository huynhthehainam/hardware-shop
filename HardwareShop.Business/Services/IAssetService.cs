

using HardwareShop.Business.Extensions;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Services
{
    public interface IAssetService
    {
        ApplicationResponse<CachedAsset> GetAssetById(long id);
    }
}