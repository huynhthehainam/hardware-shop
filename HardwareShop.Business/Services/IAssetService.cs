

using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Services
{
    public interface IAssetService
    {
        CachedAsset? GetAssetById(long id);
    }
}